import type { I18N } from "@shared/interfaces";

/** Texts of the settings step. */
export const SETTINGS_STEP_I18N: I18N = {
	en: {
		description:
			"The basics of your account. You can change any of them later in your profile.",
		language: "Language",
		language_hint: "The app switches to the chosen language right away.",
		base_currency: "Base currency",
		base_currency_hint:
			"The main currency of your finances. Amounts are shown in it.",
		base_currency_error: "Choose the base currency.",
		snapshot_day: "Snapshot day",
		snapshot_day_hint:
			"The day of the month you take a monthly snapshot on, from 1 to 31.",
		snapshot_day_error: "Enter a day from 1 to 31.",
		research_consent: "Research",
		research_consent_label:
			"I agree that my anonymised answers may be used for research",
		research_consent_hint:
			"If you agree, you will fill in a short questionnaire later.",
		continue: "Continue",
	},
	uk: {
		description:
			"Основне про ваш акаунт. Будь-що з цього можна змінити пізніше в профілі.",
		language: "Мова",
		language_hint: "Застосунок одразу перемкнеться на обрану мову.",
		base_currency: "Базова валюта",
		base_currency_hint:
			"Основна валюта ваших фінансів. Суми показуються в ній.",
		base_currency_error: "Оберіть базову валюту.",
		snapshot_day: "День зрізу",
		snapshot_day_hint:
			"День місяця, коли ви робите щомісячний зріз, від 1 до 31.",
		snapshot_day_error: "Введіть день від 1 до 31.",
		research_consent: "Дослідження",
		research_consent_label:
			"Погоджуюся, що мої знеособлені відповіді можуть використовуватися для досліджень",
		research_consent_hint: "Якщо погодитеся, згодом заповните коротку анкету.",
		continue: "Далі",
	},
};
