#include <sys/types.h> 
#include <sys/socket.h> 
#include <netinet/in.h> 
#include <stdio.h>
#include <mysql.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include <mysql/mysql.h>


MYSQL *conn;
void conectarBD() {
	
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", NULL, 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
}
void ejecutarConsultaSQL(char *sql) {
	if (mysql_query(conn, sql) != 0) {
		fprintf(stderr, "Error en la consulta: %s\n", mysql_error(conn));
		mysql_close(conn);
		exit(EXIT_FAILURE);
	}
}


int ejecutarConsulta(char *sql) {
	
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	int err = mysql_query(conn, sql);
	if (err != 0)
	{
		printf ("Error al consultar datos de la base %u %s\n",
		mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	
	resultado = mysql_store_result (conn);//el resultado es una estructura matricial
	row = mysql_fetch_row (resultado);
	
	if (row == NULL)
		printf ("No se han obtenido datos en la consulta\n");
	else
		while (row !=NULL) {
			if(row[0] != NULL){return 1;}
			// la columna 0 contiene el nombre del jugador
			//printf ("%s\n", row[0]);
			// obtenemos la siguiente fila
			row = mysql_fetch_row (resultado);
	}


		
}






int main(int argc, char *argv[])
{
	conectarBD();
	
	// Ejecutar la consulta para crear la base de datos y las tablas
	ejecutarConsultaSQL("DROP DATABASE IF EXISTS JUEGO");
	ejecutarConsultaSQL("CREATE DATABASE JUEGO");
	ejecutarConsultaSQL("USE JUEGO");
	ejecutarConsultaSQL("CREATE TABLE jugadores(ID INTEGER PRIMARY KEY AUTO_INCREMENT, NOMBRE varchar(20) not null, CONTRASENYA varchar(20)not null, PUNTOS integer, VICTORIAS integer)");
	ejecutarConsultaSQL("CREATE TABLE partidas(ID integer primary key not null, FECHA varchar(20), TIEMPO time, JUGADORES integer)");
	ejecutarConsultaSQL("CREATE TABLE relacionjp (ID integer primary key not null, ID_J integer not null, foreign key (ID_J) references jugadores(ID), ID_P integer not null, foreign key (ID_P) references partidas(ID))");
	
	// Insertar datos en las tablas
	ejecutarConsultaSQL("INSERT INTO jugadores (ID, NOMBRE, CONTRASENYA, PUNTOS, VICTORIAS) VALUES (1, 'pol', 'contra', 100, 10), (2, 'Jugador2', 'contrasenya', 200, 15), (3, 'Jugador3', 'contrasenya3', 150, 12)");
	ejecutarConsultaSQL("INSERT INTO partidas (ID, FECHA, TIEMPO, JUGADORES) VALUES (1, '03-01', '12:00:00', 3), (2, '03-02', '15:30:00', 2), (3, '03-03', '18:45:00', 1)");
	ejecutarConsultaSQL("INSERT INTO relacionjp (ID, ID_J, ID_P) VALUES (1, 1, 1), (2, 2, 1)");
	
	
	// consulta
	
	//ejecutarConsulta("SELECT DISTINCT jugadores.NOMBRE FROM (jugadores, partidas, relacionjp) WHERE relacionjp.ID_P = '1' AND relacionjp.ID_J = jugadores.ID");
	

	
	
	
	// Cargamos la base de datos mediante un fichero
	//ejecutarConsulta("source BBDD.txt");
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	char peticion[512];
	char respuesta [512];
	// INICIALITZACIONS
	// Obrim el socket
	
	if ((sock_listen = socket (AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");
	
	// Fem el bind al port
	
	memset(&serv_adr, 0, sizeof(serv_adr)); // inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	// asocia el socket a cualquiera de las IP de la màquina. 
	//htonl formatea el numero que recibe al formato necesario 
	serv_adr.sin_addr.s_addr = htonl (INADDR_ANY);
	// escucharemos en el port 9050
	serv_adr.sin_port = htons (8024);
	
	if (bind (sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind");
	
	if (listen (sock_listen, 3) < 0) 
		printf("Error en el Listen");
	
	int i;
	// Atenderemos solo 5 peticiones
	
	for (i=0;i<20;i++){
		printf ("Escuchando\n");
		
		sock_conn = accept (sock_listen, NULL, NULL);
		printf ("He recibido conexion\n");
		//sock_conn es el socket que usaremos para este cliente 
			
		//Ahora recibimos su nombre, que dejamos en buff 
		int ret = read (sock_conn, peticion, sizeof (peticion)); 
		printf ("Recibido\n");
		
		// Tenemos que a?adirle la marca de fin de string 
		// para que no escriba lo que hay despues en el buffer 
		peticion [ret]='\0';
		
		//Escribimos el nombre en la consola
	printf ("Peticion: %s\n", peticion);
		
		// vamos a ver que quieren
		char *p = strtok( peticion, "/");
		char Funcion[12];
		strcpy(Funcion, p);
		p = strtok( NULL, "/");
		char Nombre[20];
		strcpy (Nombre, p);
		p = strtok( NULL, "/");
		char Contrasenya[10];
		strcpy (Contrasenya, p);
		printf ("Nombre: %s, Contrasenya: %s\n", Nombre, Contrasenya);
		
		if (Funcion[0] == 'l') //El usuario me pide log in
		{	
			char comando[100];
			sprintf(comando, "SELECT NOMBRE FROM jugadores WHERE NOMBRE = '%s' AND CONTRASENYA = '%s'", Nombre, Contrasenya);
			
			if(ejecutarConsulta(comando) == 1){
				sprintf (respuesta, "Bienvenido de nuevo %s, su session ha sido iniciada correctamente.", Nombre);
			}
			else
				{
				sprintf (respuesta, "Ha escrito mal su usuario o Contrasenya");
				}
		}
		
		if (Funcion[0] == 's') // El usuario me pide sign up 
		{
			char comando[100];
			sprintf(comando, "SELECT NOMBRE FROM jugadores WHERE NOMBRE = '%s'", Nombre);
			if(ejecutarConsulta(comando) == 1)
				{sprintf(respuesta, "este usuario ya existe");}
			else
			{
				char comando[100];
				sprintf(comando, "INSERT INTO jugadores (NOMBRE, CONTRASENYA) VALUES ('%s', '%s')", Nombre, Contrasenya);
				ejecutarConsultaSQL(comando);
				sprintf (respuesta, "Hola, Gracias por registrarte en nuestro juego");	
			}
		}

		
		
		
		printf ("Respuesta: %s\n", respuesta);
		// Y lo enviamos
		write (sock_conn, respuesta, strlen(respuesta));
		
		// Se acabo el servicio para este cliente 
		close (sock_conn);
		}mysql_close(conn);
	
}
