using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;

public class scriptGameEngine : MonoBehaviour
{

    public GameObject PJjoueur;
    public GameObject PJDistant;
    private Transform folderGame;
    private GameObject joueur;

    //socket pour la reception et l'envoie
    private UDPSocket socketReceive;
    private UDPSocket socketSend;
    //port local
    private int portLocal;

    private int nbDePassage;//pour pas trop surchager l'envoie de requete



    // Start is called before the first frame update
    void Start()
    {
        //------------------------------------------------
        //generation du personnage
        //------------------------------------------------
        folderGame = new GameObject("Game").transform;

        joueur = Instantiate(PJjoueur);
        joueur.transform.SetParent(folderGame);


        //------------------------------------------------
        //gestion du multijoueur
        //------------------------------------------------

        //pas upnp flem se soir


        Boolean local;
        const int portServeurDeJeu = 52051;
        string IPLocal; //IP local qui à acces a internet

        //a modifier en fonction de si le jouer joue en local
        local = true;
        IPLocal = "127.0.0.1";
        


        string ipDistante;
        socketReceive = new UDPSocket();
        socketSend = new UDPSocket();
        //UPnP upnp;//obj pour l'UPnP qui s'active pas si c'est en local

        //UDP serveur local
        portLocal = 52051;//pour de vrai ce sera au minimum 52052

        if (local)
        {
            ipDistante = "127.0.0.1";
            //upnp = new UPnP("local");
        }
        else
        {
            ipDistante = "gamama.dynbug.ovh";
            //upnp = new UPnP();
        }

        int i = 0;

        try
        {
            socketSend.Client(ipDistante, portServeurDeJeu);
            //Console.WriteLine("\nConnect in " + ipDistante);
        }
        catch(FormatException)
        {/*
            while (true)
            {
                try //resolution de nom DNS
                {
                    string ipDistanteResolu = Dns.GetHostEntry(ipDistante).AddressList[i].ToString();
                    socketSend.Client(ipDistanteResolu, portServeurDeJeu);
                    Console.WriteLine("\nConnect in " + ipDistanteResolu);
                    break;
                }
                catch (SocketException)//Pas d'enregistrement trouve pour le nom de domaine donnée
                {
                    Console.WriteLine("\nUne erreur s'est produite lors de la résolution d'adresse");
                    Environment.Exit(0);
                }
                catch (NotSupportedException)//IPv6 tester, normal
                {
                    i++;
                }
                catch (IndexOutOfRangeException)//Pas d'address IPv4 pour le nom de domaine donnée
                {
                    Console.WriteLine("No address IPv4 found");
                    //exception plus tard
                    Environment.Exit(0);
                }
            }*/
        }


        //code UPnP a metre

        i = 0;
        int tentativeMax = 16;//tentative maximal pour générer un UPnP et ou un port

        do
        {
            try
            {
                i++;
                portLocal++;
                socketReceive.Server(IPLocal, portLocal);//1er catch
                //await upnp.CreationUPnP(portLocal);//2eme catch
                i = tentativeMax;
            }
            catch (SocketException)
            {
                if (i > tentativeMax)
                {
                    //Console.WriteLine("Imposibilité dobtenir un port");
                    //exception plus tard
                    //Environment.Exit(0);
                }
            }//port deja occuper sur l'ordinateur
            /*
            catch (MappingException)//port deja occuper sur le routeur UPnP
            {
                socketReceive._socket.Close();
                if (i > tentativeMax)
                {
                    Console.WriteLine("Imposibilité de créer un UPnP");
                    //exception plus tard
                    Environment.Exit(0);
                }
            }*/
        }
        while (i < tentativeMax);

        socketSend.Send("idHere" + ":" + portLocal.ToString() + ":-1:-1");

        nbDePassage = 0;


    }

    // Update is called once per frame
    void Update()
    {
        if(nbDePassage==0)
        {
            GameObject joueur2 = GameObject.Instantiate(PJDistant, transform.position, transform.rotation);
            joueur2.transform.SetParent(folderGame);
        }


        if(nbDePassage>=600)
        {
            nbDePassage = 1;
            socketSend.Send("idHere" + ":" + portLocal.ToString() + ":" + joueur.transform.position.x + ":" + joueur.transform.position.y);
        }
        nbDePassage+=1;
        
    }


    void Quitter()
    {
        socketSend.Send("exit");
        socketReceive._socket.Close();
    }
}
