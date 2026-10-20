import type { I18N } from "@shared/interfaces";

/** Texts of the step with the months before the first snapshot. */
export const ESTIMATES_STEP_I18N: I18N = {
	en: {
		description:
			"Rough amounts for {previous} and {two_months_ago}. You can round them to thousands and correct them later in the history.",
		expenses_total: "Total expenses",
		saved: "Saved",
		note: "Together with the first snapshot that makes three months, so the index is calculated right away. The history marks these months as roughs.",
		continue: "Continue",
	},
	uk: {
		description:
			"Приблизні суми за {previous} і {two_months_ago}. Можна округлити до тисяч і пізніше виправити в історії.",
		expenses_total: "Витрати разом",
		saved: "Відкладено",
		note: "Разом із першим зрізом це три місяці, тож індекс порахуємо одразу. В історії ці місяці будуть позначені як приблизні.",
		continue: "Далі",
	},
};
