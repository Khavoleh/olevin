import type { I18N } from "@shared/interfaces";

/** Texts of the time zone field. */
export const TIMEZONE_FIELD_I18N: I18N = {
	en: {
		timezone: "Time zone",
		timezone_hint: "It sets when your snapshot day starts.",
		timezone_error: "Choose a time zone from the list.",
		show_timezones: "Show time zones",
	},
	uk: {
		timezone: "Часовий пояс",
		timezone_hint: "Від нього залежить, коли настає день зрізу.",
		timezone_error: "Оберіть часовий пояс зі списку.",
		show_timezones: "Показати часові пояси",
	},
};
