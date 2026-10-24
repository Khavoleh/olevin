/** Charts of the trend page: the value they show, their texts, the ticks and the terms on the right. */
export const TREND_METRICS = [
	{
		metric: "index",
		titleKey: "index",
		captionKey: "index_caption",
		ticks: [0, 40, 70, 100],
		terms: [
			{ value: 20, labelKey: "zone_stress" },
			{ value: 55, labelKey: "zone_unstable" },
			{ value: 85, labelKey: "zone_calm" },
		],
		hasZones: true,
	},
	{
		metric: "cashFlow",
		titleKey: "cash_flow",
		captionKey: "subsystem_caption",
		ticks: [0, 50, 100],
		terms: [
			{ value: 0, labelKey: "term_weak" },
			{ value: 50, labelKey: "term_moderate" },
			{ value: 100, labelKey: "term_strong" },
		],
		hasZones: false,
	},
	{
		metric: "buffer",
		titleKey: "buffer",
		captionKey: "subsystem_caption",
		ticks: [0, 50, 100],
		terms: [
			{ value: 0, labelKey: "term_weak" },
			{ value: 50, labelKey: "term_moderate" },
			{ value: 100, labelKey: "term_strong" },
		],
		hasZones: false,
	},
] as const;
