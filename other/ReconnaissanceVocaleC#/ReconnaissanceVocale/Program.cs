using DemoRecoVocale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconnaissanceVocale
{
  class Program
  {
    static MoteurReconnaissanceVocale rV;
    static void Main(string[] args)
    {
      Console.Write("Début du test.\n\n");
      Console.Write("***********************************\n\n");
      Console.Write("Exemple de commandes : \n");
      Console.Write(" - Allumer le salon. \n");
      Console.Write(" - Eteindre la cuisne. \n");
      Console.Write(" - Fermer la chambre. \n");
      Console.Write(" - Verrouiller toutes les portes. \n");
      Console.Write("\n***********************************\n\n");
      rV = new MoteurReconnaissanceVocale(gestion, Console.Write);
      Console.Write("Dites une commande : \n");
      rV.lancerReconnaissance();
      while (true) 
      {
        rV.lancerReconnaissance();
        System.Threading.Thread.Sleep(1000);
      };
      
    }

    private static void gestion(string commande) 
    {
      if(commande.Equals("QUITTER")) {
        Console.Write("Fermeture dans :");
        for (int i = 5; i > 0; i++ )
        {
          Console.Write(i);
          System.Threading.Thread.Sleep(1000);
        }
        Environment.Exit(0);
      }
      else
      {
        Console.Write("Commande reçue : \n");
        Console.Write(commande+"\n");
      }
      Console.Write("Dites une commande : \n");

    }
  }
}
