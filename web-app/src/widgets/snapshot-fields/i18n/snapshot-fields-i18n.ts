import type { I18N } from "@shared/interfaces";

/** Texts of the fields of an actual snapshot. */
export const SNAPSHOT_FIELDS_I18N: I18N = {
	en: {
		income: "Income for the month",
		income_hint: "After-tax income.",
		expenses: "Expenses for the month",
		expenses_hint:
			"Loan payments don't go here, there is a separate field for them below.",
		food: "Food",
		housing: "Housing",
		health: "Health and medicines",
		clothing: "Clothing",
		restaurants: "Restaurants",
		transport: "Transport",
		other: "Other",
		total: "Total",
		saved: "Saved during the month",
		saved_hint: "How much you moved into savings this month.",
		saved_label: "Saved",
		reserves: "Reserves at the end of the month",
		reserves_hint:
			"Money you can use within a week: cards, cash, demand deposits.",
		debt_payments: "Debt payments",
		debt_payments_hint:
			"Monthly payments on loans and instalments. Leave 0 if you have no debts.",
		self_rating: "How do you feel about money?",
		self_rating_hint:
			"How calm you feel about your finances this month, from 1 to 5.",
		self_rating_error: "Choose a rating from 1 to 5.",
		self_rating_low: "Very anxious",
		self_rating_high: "Completely calm",
	},
	uk: {
		income: "Дохід за місяць",
		income_hint: "Сума після податків.",
		expenses: "Витрати за місяць",
		expenses_hint:
			"Платежі за кредитами сюди не входять, для них є окреме поле нижче.",
		food: "Їжа",
		housing: "Житло",
		health: "Здоров'я та ліки",
		clothing: "Одяг",
		restaurants: "Ресторани",
		transport: "Транспорт",
		other: "Інше",
		total: "Разом",
		saved: "Відкладено за місяць",
		saved_hint: "Скільки ви перевели в заощадження цього місяця.",
		saved_label: "Відкладено",
		reserves: "Резерви на кінець місяця",
		reserves_hint:
			"Гроші, якими можна скористатися протягом тижня: картки, готівка, депозити до запитання.",
		debt_payments: "Платежі за боргами",
		debt_payments_hint:
			"Щомісячні платежі за кредитами й розстрочками. Залиште 0, якщо боргів немає.",
		self_rating: "Як ви почуваєтеся щодо грошей?",
		self_rating_hint:
			"Наскільки спокійно ви почуваєтеся щодо своїх фінансів цього місяця, від 1 до 5.",
		self_rating_error: "Оберіть оцінку від 1 до 5.",
		self_rating_low: "Дуже тривожно",
		self_rating_high: "Цілком спокійно",
	},
};
