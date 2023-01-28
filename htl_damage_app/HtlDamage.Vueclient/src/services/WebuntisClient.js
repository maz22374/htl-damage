
/**
 * Facade for Webuntis client
 * see https://github.com/SchoolUtils/WebUntis (I haven't test it, please try out. Maybe it's better
 * to analyze the source code and write your own logic.)
 */
class WebuntisClient {
    constructor() {

    }
    async login(username, password) {
        if (!username || typeof username !== "string") throw new Error(`Username is not a string or empty.`);
        if (!password || typeof password !== "string") throw new Error(`Password is not a string or empty.`);

        // TODO: Authenticate
        this.authenticated = true;
    }
    async getLessonsInRoom(date, room) {
        if (!this.authenticated) throw new Error("Not authenticated.");
        if (!(date instanceof Date)) throw new Error(`Invalid date: ${date}.`);
        if (!room || typeof room !== "string") throw new Error(`Invalid room: ${room}.`);

        // TODO: Get lessons from webuntis.

        // Fake it till you make it
        return [
            { lesson: 1, day: date.getTime(), classes: "3CHIF", teachers: ["SZ", "KRB"] },
            { lesson: 2, day: date.getTime(), classes: "3CHIF", teachers: ["SZ", "KRB"] },
            { lesson: 4, day: date.getTime(), classes: ["3AHIF", "3BHIF"], teachers: ["NAI"] }
        ]
    }

    /**
     * Liefert zu einem Date Objekt die aktuelle Stunde lt. Stundenraster (8:30 -> 1, 10:12 -> 3).
     * Das ist wichtig, damit der Aufrufer bei der Schadensmeldung die Unterrichtsstunde
     * speichern kann. In der Pause ist die vorige Stunde zu liefern (9:43 -> 2).
     * Das Stundenraster kann fix im Code gepspeichert werden, damit wir keinen API Call brauchen
     * und sich das Raster sowieso nicht ändert.
     */
    getLessonNumber(date) {
        if (!(date instanceof Date)) throw new Error(`Invalid date: ${date}.`);

    }
}

const client = new WebuntisClient();

export default client;
