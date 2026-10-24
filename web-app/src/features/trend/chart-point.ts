/** Point of a trend chart: the month and year on the axis, the value and its text; no value for a month without a snapshot. */
export interface ChartPoint {
	month: string;
	year: number;
	value: number | null;
	text: string;
}
