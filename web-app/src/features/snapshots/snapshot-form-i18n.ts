import type { I18N } from "@shared/interfaces";

/** Texts of the snapshot form page. */
export const SNAPSHOT_FORM_I18N: I18N = {
	en: {
		title_new: "New snapshot",
		title_edit: "Edit snapshot",
		description_new:
			"Totals for {month}, in any of the currencies. An empty field counts as 0.",
		description_edit:
			"Amounts for {month} as you entered them. An empty field counts as 0.",
		description_estimate:
			"Rough amounts for {month} from the onboarding. You can round them to thousands.",
		month: "Month",
		month_hint: "You can fill in a month you missed.",
		closed: "It is not time for a snapshot yet. The next one opens on {date}.",
		back_to_history: "Back to the history",
		save_new: "Save the snapshot",
		save: "Save the changes",
		cancel: "Cancel",
		recalculate_title: "Recalculate the later snapshots?",
		recalculate_one:
			"The {count} later snapshot will be recalculated with the new amounts.",
		recalculate_other:
			"The {count} later snapshots will be recalculated with the new amounts.",
		recalculate_confirm: "Save and recalculate",
	},
	uk: {
		title_new: "Новий зріз",
		title_edit: "Зміна зрізу",
		description_new:
			"Підсумкові суми за {month}, у будь-якій із валют. Порожнє поле рахується як 0.",
		description_edit:
			"Суми за {month} так, як ви їх ввели. Порожнє поле рахується як 0.",
		description_estimate:
			"Приблизні суми за {month} з онбордингу. Можна округлити до тисяч.",
		month: "Місяць",
		month_hint: "Можна заповнити пропущений місяць.",
		closed: "Час робити зріз ще не настав. Наступний відкриється {date}.",
		back_to_history: "До історії",
		save_new: "Зберегти зріз",
		save: "Зберегти зміни",
		cancel: "Скасувати",
		recalculate_title: "Перерахувати новіші зрізи?",
		recalculate_one: "{count} новіший зріз буде перераховано з новими сумами.",
		recalculate_few: "{count} новіші зрізи буде перераховано з новими сумами.",
		recalculate_many:
			"{count} новіших зрізів буде перераховано з новими сумами.",
		recalculate_other:
			"{count} новішого зрізу буде перераховано з новими сумами.",
		recalculate_confirm: "Зберегти й перерахувати",
	},
};
