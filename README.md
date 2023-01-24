# Projekt HTL-Damage

Dir. Hager hat in der letzten SGA Sitzung um Ideen gebeten, die Schäden im Gebäude in den Griff
zu bekommen. Aus meiner Sicht ergibt sich ein schönes Projekt für einen 3. Jahrgang:

- Authentifizierte User (Studierende, Lehrende, Verwaltung) können mit ihrem Smartphone Schäden
  melden.
- Zuerst wird der Raum ausgewählt. Schon gemeldete Schäden werden angezeigt. Falls der Schaden
  schon gemeldet wurde, kann dieser bestätigt werden.
- Bei einem neuen Schaden können User mit ihrem Smartphone einen Schaden fotografieren.
- Schäden können in zumindest 3 Kategorien unterteilt werden:
  - Verschmutzungen (Tisch beschmiert, ...)
  - Schäden am Inventar, die Reparaturmaßnahmen erfordern (Spind zerstört, Wand beschmutzt)
  - Sicherheitskritische Schäden (Steckdose hängt heraus, ...)
- Je nach Kategorie ergeht eine Sofortmeldung per Mail an die Hausverwaltung oder wird nur
  im System gespeichert.
- Die Lehrenden haben Einsicht in die Schadensmeldung.
- Die Hausverwaltung kann eine Behebung vermerken.
- Optional: Eine Schnittstelle zu WebUntis ermittelt bei der Zuordnung, welche Klassen lt.
  Stundenplan vorher im gemeldeten Raum waren.
- Optional: Am Smartphone kann ein Bildausschnitt gewählt werden und Marker können gesetzt werden.
- Optional: Mit Methoden des Data Science wird versucht, Klassen zu ermitteln, die Schäden
  verursacht haben. Das ist experimentell und ergebnisoffen, ob so etwas funktionieren kann.

## Synchronisieren des Repositorys in einen Ordner

Installiere die neueste Version von [git](https://git-scm.com/downloads) mit den Standardeinstellungen.
Öffne dann ein Terminal, navigiere zu einem geeigneten Ordner und führe folgenden Befehl aus:

```
git clone https://github.com/maz22374/htl-damage.git
```

Um die neueste Version vom Server zu laden, führe die folgende Datei aus. Warnung: Alle lokalen Änderungen werden zurückgesetzt.

**Windows**

```
resetGit.cmd
```

**macOS, Linux**

```
chmod a+x 
./resetGit.sh
```

## Teammitglieder

| Name                        | Email                    | Aufgabenbereich                         | 
| -----------------------     | ----------------------   | --------------------------------------- |
| David *Mazurek*, 3CHIF      | maz22374@spengergasse.at | Neue Schäden melden                     |
| Viktor *Zhelev*, 3CHIF      | zhe22045@spengergasse.at | Vorhandene Schäden anzeigen             |
| Matija *Radomirovic*, 3CHIF | rad22669@spengergasse.at | WebUntis Schnittstelle                  |

