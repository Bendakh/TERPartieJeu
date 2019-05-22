#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <netdb.h>
#include <pthread.h>


//#define PORT "9994"
#define NB_CASE 10
#define NB_CAR 20
#define NB_PLAYERS 20
#define NB_SALON 10

char tab[NB_CASE][NB_CASE];

struct message
{
  int x;
  int y;
  char contenu;
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

struct player {
    int id;
    char pseudo[20];
    int x;
    int y;
    int hp;
    int nbBombe;
    int puissance;
};

//char tab[NB_PLAYERS][NB_CAR];



// Thread écriture
void* fctThreadEcriture(void* par){
  printf("%lu\n", (unsigned long int)pthread_self());
  //initialisation
  int * clientSocket  = (int *) par;
  //char choix[1];

  //struct message msg;
//int choix;
  /*
  while(1) {
    printf("\nMENU :\t0. Quitter\n\t1. Continuer\n\t2. Afficher document\n");
    char choix[1];
    msg.x=-1;
    msg.y=-1;
    scanf("%s",&choix);
    if(strcmp(choix,"1")==0) {
        while(msg.x<0||msg.x>NB_CASE){
            printf("Veuillez entrer un indice entre 0 et %d  pour x : \n", NB_CASE-1);
            scanf("%d",&msg.x);
        }
        while(msg.y<0||msg.y>NB_CASE) {
            printf("Veuillez entrer un indice entre 0 et %d  pour y: \n", NB_CASE-1);
            scanf("%d",&msg.y);
        }
        msg.contenu = 'j';
        send(*clientSocket,&msg,sizeof(char) + sizeof(int)*2 ,0);
    } else if(strcmp(choix,"0")==0){
      close(*clientSocket);
      //printf("\nMerci pour la contribution, à bientôt\n");
      pthread_exit(NULL);
    } else if(strcmp(choix,"2")==0){
        for (int i = 0; i < NB_CASE; i++)
        {
            for (int t = 0; t < NB_CASE; ++t)
            {
                printf(" ---");
            }
            printf("\n");
            for (int j = 0; j < NB_CASE; j++)
            {
                printf("| %c ",tab[i][j]);
            }
            printf("|\n");
        }
        for (int t = 0; t < NB_CASE; ++t)
        {
            printf(" ---");
        }
        printf("\n");
    }
    else{
      printf("Choix incorrecte, veuillez réessayer !\n");
    }

    }*/
    int id=1;
    struct message_recv msg;
    msg.x =-1;
    msg.y =-2;
    msg.c_y=3;
    msg.c_x=4;
    msg.direction = 'c';

    while(1) {
      //sleep(1);
      //scanf("%s",&choix);
      send(*clientSocket , &id, sizeof(int), 0);

      send(*clientSocket ,&msg, sizeof(struct message_recv), 0);
    }

  //pthread_exit(NULL);
}

// Thread affichage mise-à-jour
void* fctThreadMAJ(void* par){

  //Initialisation
  int * clientSocket  = (int *) par;
  //struct message msg;
  struct message_send msg;
  struct player player;
  int id;
  //traitement : attente de modification et affichage
  while(1) {
    //char tab[NB_CASE];
    int n = 0;
    int taille =0;

    taille = recv(*clientSocket,&id, sizeof(int),0);
        if (taille==-1) {
            perror(" perror recv from client id");
            close(*clientSocket);
            pthread_exit(NULL);
        } else if(taille==0) {
            printf("Client déconnecté\n");
            close(*clientSocket);
            pthread_exit(NULL);
        }

      printf("ID : %d\n",id );
    if(id == 4) {
      do {
        taille=recv(*clientSocket,&player+n, sizeof(struct player) - n, 0);
        n += taille;
      } while (n > 0 && n < sizeof(char) + sizeof(int)*2 - 1 );
      if(taille>0){
          //printf("MAJ - Thread maj : x = %d | y = %d | c_x = %d | c_y = %d | direction =  %c | player id = %d\n",msg.x,msg.y, msg.c_x, msg.c_y, msg.direction, msg.id);
          printf("%d %s %d %d %d %d %d\n", player.id, player.pseudo, player.x, player.y, player.hp, player.nbBombe, player.puissance);
      }
      else if(taille==-1){
        perror(" perror recv from client");
        exit(1);
      }
      else if(taille==0){
        printf("Serveur déconnecté\nDéconnexion en cours...\nMerci pour la contribution, à bientôt.\n");
        close(*clientSocket);
        //pthread_exit(NULL);
        exit(1);
      }
    }
    if(id == 1) {
      do {
        taille=recv(*clientSocket,&msg+n, sizeof(struct message_send) - n, 0);
        n += taille;
      } while (n > 0 && n < sizeof(char) + sizeof(int)*2 - 1 );
      if(taille>0){
          printf("MAJ - Thread maj : x = %d | y = %d | c_x = %d | c_y = %d | direction =  %c | player id = %d\n",msg.x,msg.y, msg.c_x, msg.c_y, msg.direction, msg.id);
      }
      else if(taille==-1){
        perror(" perror recv from client");
        exit(1);
      }
      else if(taille==0){
        printf("Serveur déconnecté\nDéconnexion en cours...\nMerci pour la contribution, à bientôt.\n");
        close(*clientSocket);
        //pthread_exit(NULL);
        exit(1);
      }
    }
  }
//   pthread_exit(NULL);
}


int main (int argc, char *argv[]) {

  /************************************************************************/
  /*****************  PARTIE RESEAU : CONNEXION AU SERVEUR  ***************/
  /************************************************************************/
  int portSer;
  char adSer[16];
  if(argc!=4){
    printf("Exécuter correctement : ./client <adress_serv> <port_serv> 1/0\n");
    exit(1);
  }
  else{
    strcpy(adSer,argv[1]);
    portSer = atoi(argv[2]);
  }
  
  int isSender;
  isSender = atoi(argv[3]);

	/************************************************************************/
	/********************  PARTIE RESEAU : INITIALISATION  ******************/
	/************************************************************************/
	//CREATION DE LA SOCKET
	int client_socket = socket(PF_INET,SOCK_STREAM, 0);   //creation de socket TCP
	if (client_socket<0) {
		perror("Erreur creation socket client");
    exit(1);
	}
	//addr du server
	struct sockaddr_in aS;
	aS.sin_family = AF_INET ;
	//inet_pton(AF_INET,adSer,&(aS.sin_addr)); //Adresse du serveur
	aS.sin_addr.s_addr=inet_addr(adSer);
  	aS.sin_port = htons(portSer) ;  //port
	socklen_t lgA = sizeof(struct sockaddr_in);


	/******************** CONNECTION  ******************/
	printf("Demande de connexion au %s:%d \n",adSer,portSer);
  int echec = connect(client_socket,(struct sockaddr *) &aS,lgA);
  if (echec<0){
  	perror("Erreur de connexion");
    exit(1);
  }
  printf("Connexion autorisée\n\n**************\n** Bienvenue **\n**************\n\n");
    /*
    int n = 0;
    int taille =0;
    do {
        taille=recv(client_socket,&tab-n, sizeof(tab) - n, 0);
        n += taille;
    } while (n > 0 && n < sizeof(tab) - 1 );
    if(taille>0){
        printf("Joueurs : \n");
        int j=0;
        while(strcmp(tab[j],"") != 0) {
            printf("- %s\n",tab[j]);
            j++;
        }
    } else {
        printf("TOTO\n");
    }
    printf("Votre pseudo : \n");
    char pseudo[20];
    scanf("%s", pseudo);
    printf("oui : %s\n", pseudo);
    send(client_socket,&pseudo,sizeof(char)*20,0);
    */

  int n = 0;
  int taille = 0;
  //------------------------TEST----------------
  //int id = 0;
  //struct message msg;

  //recv(client_socket,&id, sizeof(int),0);
  /*
  do {
        taille=recv(client_socket,&msg-n, sizeof(char) + sizeof(int)*2 - n, 0);
        n += taille;
    } while (n > 0 && n < sizeof(char) + sizeof(int)*2 - 1 );
    if(taille>0){
        //doc->c_maj_x = msg.x;
        //doc->c_maj_y = msg.y;
        //doc->tab[msg.x][msg.y] = msg.contenu;
        switch (id)
        {
        case 1 :
            printf("CAS UN : %d %d %c\n", msg.x, msg.y, msg.contenu);
            break;
        case 2 :
            printf("CAS DEUX : %.6f\n", msg.x);
            break;
        }

    }*/

   //------------------------------------------
  /*
  do {
    taille=recv(client_socket,&tab-n, sizeof(tab) - n, 0);
    n += taille;
  } while (n > 0 && n < sizeof(tab) - 1 );
  if(taille>0){
    for (int i = 0; i < NB_CASE; i++)
    {
        for (int t = 0; t < NB_CASE; ++t)
        {
            printf(" ---");
        }
        printf("\n");
        for (int j = 0; j < NB_CASE; j++)
        {
            printf("| %c ",tab[i][j]);
        }
        printf("|\n");
    }
    for (int t = 0; t < NB_CASE; ++t)
    {
        printf(" ---");
    }
    printf("\n\n");

  }
  else if(taille==-1){
    perror(" perror recv from server");
    exit(1);
  }
  else if(taille==0){
    printf("Serveur déconnecté\n");
    exit(1);
  }
  */

    pthread_t threadEcriture;
    pthread_t threadMAJ;

    // // Création et lancement des threads
    if(pthread_create(&threadMAJ,NULL,fctThreadMAJ,&client_socket)!=0)
    	perror("Erreur création du thread MAJ");

	if (isSender == 1) {
		printf("SENDER !!\n");
		if(pthread_create(&threadEcriture,NULL,fctThreadEcriture,&client_socket)!=0)
    		perror("Erreur création du thread Ecriture");
	}
	
    pthread_join(threadMAJ,NULL);

    printf("\nDéconnexion en cours...\nMerci pour la contribution, à bientôt.\n");
}