import type { I18N } from "@shared/interfaces";

/** Texts of the trend page. */
export const TREND_I18N: I18N = {
	en: {
		title: "Trend",
		period: "Period",
		six_months: "6 mo.",
		twelve_months: "12 mo.",
		all: "All",
		index: "Index",
		cash_flow: "Cash flow",
		buffer: "Financial buffer",
		index_caption:
			"Background zones: stress 0–40 · unstable 40–70 · calm 70–100. A break in the line is a month without a snapshot.",
		subsystem_caption:
			"Subsystem scale: 0 weak · 50 moderate · 100 strong. A break in the line is a month without a snapshot.",
		zone_stress: "stress",
		zone_unstable: "unstable",
		zone_calm: "calm",
		term_weak: "weak",
		term_moderate: "moderate",
		term_strong: "strong",
		empty_six_months:
			"No snapshots in the last 6 months. Choose a longer period.",
		empty_twelve_months: "No snapshots in the last 12 months. Choose “All”.",
		empty_all: "No snapshots yet.",
		one_snapshot:
			"This period has one snapshot. The line will appear with the second one.",
	},
	uk: {
		title: "Тренд",
		period: "Період",
		six_months: "6 міс.",
		twelve_months: "12 міс.",
		all: "Усе",
		index: "Індекс",
		cash_flow: "Грошовий потік",
		buffer: "Фінансовий буфер",
		index_caption:
			"Зони на фоні: стрес 0–40 · нестабільно 40–70 · спокій 70–100. Розрив лінії — місяць без зрізу.",
		subsystem_caption:
			"Шкала підсистеми: 0 слабкий · 50 помірний · 100 сильний. Розрив лінії — місяць без зрізу.",
		zone_stress: "стрес",
		zone_unstable: "нестабільно",
		zone_calm: "спокій",
		term_weak: "слабкий",
		term_moderate: "помірний",
		term_strong: "сильний",
		empty_six_months:
			"За останні 6 місяців зрізів немає. Оберіть довший період.",
		empty_twelve_months: "За останні 12 місяців зрізів немає. Оберіть «Усе».",
		empty_all: "Зрізів ще немає.",
		one_snapshot: "У цьому періоді один зріз. Лінія з'явиться з другим.",
	},
};
