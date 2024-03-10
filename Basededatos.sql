DROP DATABASE IF EXISTS JUEGO
CREATE DATABASE JUEGO
USE JUEGO
CREATE TABLE jugadores(ID INTEGER PRIMARY KEY AUTO_INCREMENT, NOMBRE varchar(20) not null, CONTRASENYA varchar(20)not null, PUNTOS integer, VICTORIAS integer)



CREATE TABLE partidas(ID integer primary key not null, FECHA varchar(20), TIEMPO time, JUGADORES integer)

CREATE TABLE relacionjp (ID integer primary key not null, ID_J integer not null, foreign key (ID_J) references jugadores(ID), ID_P integer not null, foreign key (ID_P) references partidas(ID))
	
INSERT INTO jugadores (ID, NOMBRE, CONTRASENYA, PUNTOS, VICTORIAS) VALUES (1, 'pol', 'contra', 100, 10), (2, 'Jugador2', 'contrasenya', 200, 15), (3, 'Jugador3', 'contrasenya3', 150, 12)
INSERT INTO partidas (ID, FECHA, TIEMPO, JUGADORES) VALUES (1, '03-01', '12:00:00', 3), (2, '03-02', '15:30:00', 2), (3, '03-03', '18:45:00', 1)
INSERT INTO relacionjp (ID, ID_J, ID_P) VALUES (1, 1, 1), (2, 2, 1)