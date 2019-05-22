#include <stdio.h>
#include <stdlib.h>
#include <errno.h>
#include <sys/types.h>
#include <sys/ipc.h>
#include <sys/shm.h>
#include <sys/sem.h>
#include <pthread.h>
#include <string.h>
#include <unistd.h>
#include <signal.h>

#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>


#define NB_CASE 10
#define NB_PLAYERS 20
#define NB_CAR 20
#define NB_SALON 10

// structure partagée avec tous les clients
struct documentPartage
{
    int x; //STRUCT MOUV
    int y;
    char direction;
    int c_maj_x;
    int c_maj_y;
    int range;
    int c_maj_id; //ID
    int id_player; //ID PLAYER

    char tab[NB_CASE][NB_CASE];
    int maj;
    int countNbPlayers;
    int IDJOUEUR;
    int id_bombe;
};

struct player {
    int id;
    char pseudo[20];
    int x;
    int y;
    int hp;
    int nbBombe;
    int puissance;
};


struct listPlayers
{
  int salon;
  int nbJoueurs;
  char tab[NB_PLAYERS][NB_CAR];
};

struct message_recv
{
  int x;
  int y;
  int c_x;
  int c_y;
  char direction;
};

struct message_send
{
  int id;
  int x;
  int y;
  int c_x;
  int c_y;
  char direction;
};

struct bomb_envoyer_par_le_serveur {
    int id_bombe; // tu vas gérer un truc avec les id de bombe après on aura peut être pas besoin mais pour l'instant vaut mieux savoir, pour quand faire exploser après
    int x;
    int y;
};

struct bomb_recu_par_le_serveur {
    int x;
    int y;
    int range;
};

struct bomb_explosion_envoyer_par_le_serveur {
    int id_bombe; // Soit on identifie la bombe à faire exploser par son id bombe, soit on le fait par sa position donc pour l'instnat on laisse comme ça
    int x;
    int y;
};

// structure qu'utilise les threads pour se transmettre les infos
struct threadInfo {
  int cltSocket;
  struct documentPartage * doc;
  struct player *player;
  pthread_mutex_t *lock;
  pthread_cond_t *cond;
};

struct info {
    int clientSocket;
    pthread_cond_t *cond;
    pthread_mutex_t *lock;
    struct documentPartage * doc;
};

struct infoBomb {
    int clientSocket;
    struct bomb_envoyer_par_le_serveur * bomb;
};

struct player tabPlayers[NB_PLAYERS];


void* fctThreadBomb(void * par) {
    //Initialisation
    struct infoBomb * infoB = (struct infoBomb *) par;
    int clientSocket = infoB->clientSocket;
    struct bomb_envoyer_par_le_serveur * bomb;
    bomb = infoB->bomb;
    int id = 7;
    printf("THREAD BOMB %d %d %d %d %d\n", id, clientSocket, bomb->x, bomb->y, bomb->id_bombe);
    sleep(3);
    send(clientSocket,&id,sizeof(int), 0);
    send(clientSocket,bomb, sizeof(struct bomb_envoyer_par_le_serveur), 0);

    pthread_exit(NULL);
}

// Thread écriture
void* fctThreadEcriture(void* par){

    //initialisation
    struct threadInfo * threadInfo = (struct threadInfo *) par;
    int clientSocket = threadInfo->cltSocket;
    struct documentPartage * doc = (struct documentPartage *) threadInfo->doc;

    //Traitement
    while(1) {
        int n = 0;
        int taille =0;
        int id = 0;
        struct message_recv msg1;
        struct bomb_recu_par_le_serveur bomb;

        taille = recv(clientSocket,&id, sizeof(int),0);
        if (taille==-1) {
            perror(" perror recv from client id");
            close(clientSocket);
            pthread_exit(NULL);
        } else if(taille==0) {
            printf("Client déconnecté\n");
            close(clientSocket);
            pthread_exit(NULL);
        }

        switch (id)
        {
            case 1 :
                do {
                    taille=recv(clientSocket,&msg1-n, sizeof(struct message_recv) - n, 0);
                    n += taille;
                } while (n > 0 && n < sizeof(char) + sizeof(int)*2 - 1 );
                if(taille>0){
                    //LOCK
                    pthread_mutex_lock(threadInfo->lock);
                    doc->c_maj_x = msg1.c_x;
                    doc->c_maj_y = msg1.c_y;
                    doc->x = msg1.x;
                    doc->y = msg1.y;
                    doc->direction = msg1.direction;
                    doc->c_maj_id = id;
                    doc->id_player = threadInfo->player->id;
                    threadInfo->player->x = msg1.x;
                    threadInfo->player->y = msg1.y;
                    //doc->tab[msg1.x][msg1.y] = msg1.contenu;
                    printf("On broadcast pour %u players\n", doc->countNbPlayers);
                    printf("CAS UN : %d %d %d %d %c\n", msg1.x, msg1.y, msg1.c_x, msg1.c_y, msg1.direction);
                    //REVEIL
                    pthread_cond_broadcast(threadInfo->cond);
                    //UNLOCK
                    pthread_mutex_unlock(threadInfo->lock);
                }
                else if(taille==-1){
                    perror(" perror recv from client");
                  exit(1);
                }
                else if(taille==0){
                  printf("Client déconnecté\n");
                    close(clientSocket);
                    pthread_exit(NULL);
                }
                break;
            case 6 :
                do {
                    taille=recv(clientSocket,&bomb-n, sizeof(struct bomb_recu_par_le_serveur) - n, 0);
                    n += taille;
                } while (n > 0 && n < sizeof(struct bomb_recu_par_le_serveur) - 1 );
                if(taille>0){

                    pthread_mutex_lock(threadInfo->lock);
                    doc->x = bomb.x;
                    doc->y = bomb.y;
                    doc->range = bomb.range;
                    doc->c_maj_id = id;
                    doc->id_player = threadInfo->player->id;
                    doc->id_bombe++;
                    printf("%d\n", bomb.x);
                    pthread_cond_broadcast(threadInfo->cond);
                    pthread_mutex_unlock(threadInfo->lock);
                }
                else if(taille==-1){
                    perror(" perror recv from client");
                  exit(1);
                }
                else if(taille==0){
                  printf("Client déconnecté\n");
                    close(clientSocket);
                    pthread_exit(NULL);
                }
                break;
            default:
                printf(" JAI PAS COMPRIS RECV %d\n", id);
                break;
        }
    }
}

// Thread affichage mise-à-jour
void* fctThreadMAJ(void* par){

  //Initialisation
  struct threadInfo * threadInfo = (struct threadInfo *) par;
  int clientSocket = threadInfo->cltSocket;
  struct documentPartage * doc = (struct documentPartage *) threadInfo->doc;

  struct message_send msg;
  struct bomb_envoyer_par_le_serveur bomb;
  struct infoBomb infoB;

  pthread_t threadBomb;

  //traitement : attente de modification et affichage
    while(1) {
        //LOCK
        pthread_mutex_lock(threadInfo->lock);
        printf("fctThreadMAJ après lock %lu\n",(unsigned long int)pthread_self());
        printf("Processus fils %lu : mise-à-jour en attente \n",(unsigned long int)pthread_self());
        printf("CLIENT ID : %d\n",clientSocket);
        //WAIT
        pthread_cond_wait(threadInfo->cond, threadInfo->lock);
        printf("Processus fils %lu : après wait \n",(unsigned long int)pthread_self());
//        printf("thread fils %lu : mise-à-jour en attente\n",(unsigned long int)pthread_self());
        switch (doc->c_maj_id)
        {
            case 1:
                if(doc->id_player != threadInfo->player->id) {
                    printf("Cas 1\n");
                    msg.x=doc->x;
                    msg.y=doc->y;
                    msg.c_x = doc->c_maj_x;
                    msg.c_y = doc->c_maj_y;
                    msg.direction = doc->direction;
                    msg.id = doc->id_player;
                    if( send(clientSocket,&doc->c_maj_id,sizeof(int), 0) == -1) {
                        doc->countNbPlayers--;
                        pthread_mutex_unlock(threadInfo->lock);
                        printf("EXITTT\n");
                        pthread_exit(NULL);
                    }
                    send(clientSocket,&msg,sizeof(struct message_send), 0);
                    printf("MAJ - Thread maj : x = %d | y = %d | c_x = %d | c_y = %d | direction =  %c | player id = %d\n",msg.x,msg.y, msg.c_x, msg.c_y, msg.direction, msg.id);
                }
                break;
            case 4:
                if(doc->id_player != threadInfo->player->id) {
                    printf("Cas 4\n");
                    if( send(clientSocket,&doc->c_maj_id,sizeof(int), 0) == -1) {
                        doc->countNbPlayers--;
                        pthread_mutex_unlock(threadInfo->lock);
                        printf("EXITTT\n");
                        pthread_exit(NULL);
                    }
                    printf("Nombre de joueurs : %d\n", doc->countNbPlayers);
                    printf("Nouveau joueur : %d %s %d %d %d %d %d\n", tabPlayers[doc->countNbPlayers-1].id, tabPlayers[doc->countNbPlayers-1].pseudo, tabPlayers[doc->countNbPlayers-1].x, tabPlayers[doc->countNbPlayers-1].y, tabPlayers[doc->countNbPlayers-1].hp, tabPlayers[doc->countNbPlayers-1].nbBombe, tabPlayers[doc->countNbPlayers-1].puissance);
                    send(clientSocket,&tabPlayers[doc->countNbPlayers-1], sizeof(struct player), 0);
                }
                break;
            case 6:
                printf("Cas 6\n");
                bomb.x = doc->x;
                bomb.y = doc->y;
                bomb.id_bombe = doc->id_bombe;
                infoB.bomb = &bomb;
                infoB.clientSocket = clientSocket;
                if( send(clientSocket,&doc->c_maj_id,sizeof(int), 0) == -1) {
                    doc->countNbPlayers--;
                    pthread_mutex_unlock(threadInfo->lock);
                    printf("EXITTT\n");
                    pthread_exit(NULL);
                }
                printf("Nouvelle bombe : %d %d %d\n", bomb.x, bomb.y, bomb.id_bombe);
                send(clientSocket,&bomb, sizeof(struct bomb_envoyer_par_le_serveur), 0);
                // Création et lancement des threads
                if(pthread_create(&threadBomb,NULL,fctThreadBomb,&infoB)!=0){
                    perror("Erreur création du thread MAJ");
                    exit(1);
                }

                break;
            default:
                printf(" JAI PAS COMPRIS SEND %d \n", doc->c_maj_id);
                break;
        }
        //UNLOCK
        pthread_mutex_unlock(threadInfo->lock);
    }

    pthread_exit(NULL);
}

void* jobServeurFils(void * par) {
    // Tout ce que les professus fils doivent faire
    printf("********************** THREAD START ***************************\n");
    struct info * info = (struct info *) par;

    pthread_t threadEcriture;
    pthread_t threadMAJ;
    struct threadInfo threadInfo;

    //struct documentPartage * doc = (struct documentPartage *) threadInfo->doc;
    threadInfo.doc = info->doc;
    threadInfo.cltSocket = info->clientSocket;
    threadInfo.cond = info->cond;
    threadInfo.lock = info->lock;

    int clientSocket = threadInfo.cltSocket;

    int id=3;

    //send(clientSocket, &id, sizeof(int), 0);

    //send(clientSocket,doc->tab, sizeof(doc->tab), 0);

    struct player newPlayer;
    newPlayer.id = info->doc->IDJOUEUR;
    sprintf(newPlayer.pseudo, "yoann%u", info->doc->countNbPlayers);
    newPlayer.x = 3;
    newPlayer.y = 4;
    newPlayer.hp = 20;
    newPlayer.nbBombe = 2;
    newPlayer.puissance = 3;
    tabPlayers[info->doc->countNbPlayers] = newPlayer;
    threadInfo.player = &newPlayer;
    info->doc->countNbPlayers++;
    info->doc->IDJOUEUR++;

    id = 4;

    for (int i = 0; i < info->doc->countNbPlayers-1; i++)
    {
        send(clientSocket,&id, sizeof(int), 0);
        printf("%d %s %d %d %d %d %d\n", tabPlayers[i].id, tabPlayers[i].pseudo, tabPlayers[i].x, tabPlayers[i].y, tabPlayers[i].hp, tabPlayers[i].nbBombe, tabPlayers[i].puissance);
        send(clientSocket,&tabPlayers[i], sizeof(struct player), 0);
    }

    if(pthread_create(&threadEcriture,NULL,fctThreadEcriture,&threadInfo)!=0){
        perror("Erreur création du thread Ecriture");
        exit(1);
    }


    // Création et lancement des threads
    if(pthread_create(&threadMAJ,NULL,fctThreadMAJ,&threadInfo)!=0){
        perror("Erreur création du thread MAJ");
        exit(1);
    }

    pthread_mutex_lock(threadInfo.lock);
    threadInfo.doc->c_maj_id = 4;
    threadInfo.doc->id_player = threadInfo.player->id;
    pthread_cond_broadcast(threadInfo.cond);
    pthread_mutex_unlock(threadInfo.lock);


    pthread_join(threadEcriture,NULL);
    pthread_join(threadMAJ,NULL);
    printf("COUNT AFTER : %d\n", info->doc->countNbPlayers);
    printf("FIN FILS = %lu BYE BYE %d\n", (unsigned long int) pthread_self(), clientSocket);
    pthread_exit(NULL);
}


int main(int argc, char const *argv[])
{
    int PORT;
    char ip[16];
    if(argc!=3){
        printf("Reessayer comme ça : ./serveur adress_serv port_serv\n");
        exit(1);
    }
    else{
        strcpy(ip,argv[1]);
        PORT = atoi(argv[2]);
    }


    //CREATION DE LA SOCKET
    int serv_socket = socket(PF_INET,SOCK_STREAM, 0);   //creation de socket TCP
    if (serv_socket<0){
        perror("Erreur creation socket serveur");
        exit(1);
    }

    struct sockaddr_in ad;
    ad.sin_family = AF_INET;
    ad.sin_addr.s_addr=inet_addr(ip);
    ad.sin_port=htons(PORT);

    //Nommage de la socket
    int binded = bind(serv_socket,(struct sockaddr*)&ad,sizeof(ad));
    if(binded < 0) {
        perror("Error bind");
        exit(1);
    }

    int listened = listen(serv_socket,100);
    if(listened < 0) {
        perror("Error listen");
        exit(1);
    }


  /************************************************************************/
  /******************** LANCEMENT DES THREADS *****************************/
  /************************************************************************/


    pthread_t threadStart;
    struct info info;
    struct documentPartage doc;

    for (int i = 0; i < 10; i++)
    {
        for (int j = 0; j < 10; j++)
        {
            if(j%2 == 0) {
                doc.tab[i][j] = 'x';
            } else if(i%2 == 0) {
                doc.tab[i][j] = 'y';
            } else {
                doc.tab[i][j] = 'e';
            }
        }
    }
    doc.maj = 0;
    doc.countNbPlayers = 0;
    doc.IDJOUEUR = 0;
    doc.id_bombe = 0;


    //Cond & Lock
    pthread_cond_t cond = PTHREAD_COND_INITIALIZER;
    pthread_mutex_t lock = PTHREAD_MUTEX_INITIALIZER;

    info.cond = &cond;
    info.lock = &lock;
    info.doc = &doc;


  while(1) {

    struct sockaddr_in clientAddr;
    socklen_t lg =sizeof(struct sockaddr_in);

    // Accepter
    int clientSocket =accept(serv_socket,(struct sockaddr*) &clientAddr,&lg);
    if (clientSocket<0) {
      perror("Error accept");
      exit(1);
    }

    printf("NOUVEAU CLIENT \n");

    info.clientSocket = clientSocket;

    if(pthread_create(&threadStart,NULL,jobServeurFils,&info)!=0)
    perror("Erreur création du thread MAJ");

  }
  return 0;
}