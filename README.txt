/* Deze readme is geschreven door Lennart de Waart (563079) */

Hoe zorg je dat de solution werkt?
	- Zorg dat je ASP.NET Core 2.2 op je computer kunt draaien
	- Zorg dat je SQL Express 2017 (https://www.microsoft.com/nl-nl/sql-server/sql-server-editions-express) hebt geïnstalleerd zodat je
	  een lokale database kunt aanmaken
	- De migrations staan al klaar, dus je hoeft geen add-migrations meer uit te voeren. Alleen update-database (evt. -V)

Codeerafspraken:
	- Er is een voorbeeld call gemaakt in de AvailabilitiesController voor het ophalen van alle Availability records. Gebruik deze methode als 
	  lijdraad voor het maken van andere calls
	- Commentaar in het Engels
	- Boven iedere methode een /// <summary></summary> met daarin de functie van de methode
	- Houdt de gecreëerde structuur aan. Ga niet zomaar iets veranderen/verwijderen zonder het team op de hoogte te stellen
	- Als je een package moet installeren stel je daarvan het team op de hoogte
	- Ga niet zomaar iets lopen aanpassen in de .json bestanden, het startup bestand of in de modelbestanden
	- Alle http fout responses in het Nederlands zodat de Mobile-groep deze eventueel kan gebruiken in de applicatie
	- Verander het migrations InitialCreate bestand niet! Het enige wat je hoeft te doen om de database te creëren is het commando update-database
	  add-migration hoeft dus niet gedaan te worden
	- De controller methoden van de api bevatten nauwelijk logica. Alle logica wordt gedaan in het BusinessLogic project onder bijvoorbeeld de map Services

Git-afspraken:
	- Werk alleen in je eigen branch. Niemand werkt zonder goedkeuring van het team in de master branch.
	- Push alleen naar je eigen branch
	- Geen pullrequests op de master branch zonder goedkeuring van het team
	- Geen syncrequests op de master branch zonder goedkeuring van het team
	- Probeer zo helder en functioneel mogelijk commentaar te leveren bij iedere push/commit
	- Commentaar op pushes bij voorkeur in het Engels

	Gegevens Azure SQL-database:
	Servernaam: bezorgdirect-bezorgersapplicatie.database.windows.net
	Aanmeldgegevens serverbeheerder: bezorgdirect-bezorgersapi
	Wachtwoord: Yf4!Hfw0CagklxzH
	LET OP!! Om de database te kunnen benaderen moet je IP-adres gewhitelist zijn in Azure.
