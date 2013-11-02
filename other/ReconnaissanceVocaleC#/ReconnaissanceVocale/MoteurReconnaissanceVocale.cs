using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition.SrgsGrammar;
using System.Speech.Recognition;


namespace DemoRecoVocale
{
  class MoteurReconnaissanceVocale
  {
    // Moteur de la reconnaissance
    public SpeechRecognitionEngine moteur;

    // Type des function déléguée
    public delegate void Del(string message);
    // Function appelée après reconnaissance
    private Del gereCommande;
    private Del gerePhrase;

    //Permet de savoir si une reconnaissance est en cours
    private bool enReconnaissance;


    // Constructeur
    // gereCommande vas recevoir en parametre la chaine générer après traitement
    // gerePhrase vas recevoir la phrase reconnue sans traitment
    public MoteurReconnaissanceVocale(Del gC ,  Del gP )
    {
      // On sauvegarde les fonction en paramètre
      this.gereCommande = gC;
      this.gerePhrase = gP;

      // Pas en reconnaissance
      enReconnaissance = false;

      // Ce fichier contiendra la grammaire reconnaissable par l'application
      SrgsDocument xmlGrammar = new SrgsDocument("Grammaire.xml");
      Grammar grammar = new Grammar(xmlGrammar);

      // Chargement du moteur
      moteur = new SpeechRecognitionEngine();

      // Récupération du son périphérique audio par défaut
      moteur.SetInputToDefaultAudioDevice();

      // Charge la grammaire
      moteur.LoadGrammar(grammar);

      // Relie les fonctions
      moteur.SpeechRecognized += ParoleReconnue;
      moteur.SpeechRecognitionRejected += ParoleRejetee;

      // Permet de récupérer plusieurs solutions reconnues
      moteur.MaxAlternates = 4;
    }

    // Méthode permettant de récupérer une commande
    public void lancerReconnaissance()
    {
      if(!enReconnaissance)
      {
        enReconnaissance = true;       
        this.moteur.RecognizeAsync(RecognizeMode.Single);
      }
    }


    // Méthode appélée quand la reconnaissance à raté
    // C'est à dire que le logiciel n'a reconnu aucune phrase de la grammaire
    private void ParoleRejetee(object sender, SpeechRecognitionRejectedEventArgs e)
    {
      this.enReconnaissance = false;
      this.gereCommande("REJETEE");
    }

    /// Méthode utilisée lorsque la reconnaissance a réussi
    private void ParoleReconnue(object sender, SpeechRecognizedEventArgs e)
    {
      this.enReconnaissance = false;
      string commande = "";
      // Récupération de la commande de base utilisée (QUITTER ou une action a effectuer)
      commande = e.Result.Semantics["REQUEST"].Value.ToString();
      // On demande d'arreter
      if (commande.Equals("QUITTER"))
      {
        this.gereCommande(commande);
      }
      // On effectue une autre requete de la grammaire
      else
      {
        if (e.Result.Semantics.ContainsKey("PIECE"))
        {
          string piece = e.Result.Semantics["PIECE"].Value.ToString();
          commande += ":" + piece;
          try
          {   
            //Parcours des alternatives pour toutes les afficher
            for (int i = 0; i < e.Result.Alternates.ToArray().Length; i++)
            {
              string commandeTmp = e.Result.Alternates.ToArray()[i].Semantics["PIECE"].Value.ToString();
              if (i != 0)
              {
                commande += "|"+commandeTmp;
              }
            }
          }
          catch { }
        }
        // Nous n'avons pas trouvée le parametre
        else
        {
          commande += ":NULL";
        }
      }
      this.gerePhrase(e.Result.Text+"\n");
      this.gereCommande(commande);
    }
  }
}