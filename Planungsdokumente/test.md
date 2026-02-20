## ID: T01 – Kunde anlegen und in Kundenliste sichtbar
**Beschreibung:** Prüft, ob ein neuer Kunde über die GUI angelegt wird und danach in der Kundenübersicht erscheint.
**Vorbedingungen:** Anwendung läuft, Fenster „KundeFenster“ ist geöffnet.
**Test-Schritte:**
1. Button zum Anlegen eines neuen Kunden öffnen (z. B. „Neu“/„Hinzufügen“).
2. Name: `Max Mustermann` eintragen.
3. Adresse: `Teststraße 1` eintragen.
4. E-Mail: `max@example.com` eintragen.
5. Telefon: `0123-4567` eintragen.
6. „Speichern“ klicken.
7. In der Kundenliste prüfen, ob `Max Mustermann` angezeigt wird.
**Erwartetes Resultat:**
- Kunde wird gespeichert.
- Kunde erscheint in der Kundenliste.
- Keine Fehlermeldung, kein Absturz.


## ID: T02 – Kunde bearbeiten und Änderung wird gespeichert
**Beschreibung:** Prüft, ob ein bestehender Kunde bearbeitet und die Änderung dauerhaft gespeichert wird.
**Vorbedingungen:** Anwendung läuft, mindestens ein Kunde existiert, Fenster „KundeFenster“ ist geöffnet.
**Test-Schritte:**
1. Einen bestehenden Kunden in der Kundenliste auswählen.
2. Button „Bearbeiten“ öffnen.
3. Im Feld „Adresse“ den Wert ändern (z. B. zu `Neue Straße 99`).
4. „Speichern“ klicken.
5. Kundenliste aktualisieren (falls nötig Fenster schließen/öffnen oder Refresh/Neu laden).
6. Prüfen, ob die Adresse beim Kunden nun `Neue Straße 99` ist.
**Erwartetes Resultat:**
- Änderung wird gespeichert.
- Nach dem Neuladen ist die neue Adresse weiterhin vorhanden.
- Keine Fehlermeldung, kein Absturz.
