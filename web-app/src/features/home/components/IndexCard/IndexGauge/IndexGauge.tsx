"use client";

import { PieChart } from "recharts";
import type { Zone } from "../../../api/home.schemas";
import GaugeArcs from "./GaugeArcs";
import GaugeNumber from "./GaugeNumber";

interface IndexGaugeProps {
	value: number;
	zone: Zone;
	valueText: string;
	outOfText: string;
	label: string;
}

/** Half-circle gauge of the index: the zones, the value in the color of its zone and the number under it. */
const IndexGauge = ({
	value,
	zone,
	valueText,
	outOfText,
	label,
}: Readonly<IndexGaugeProps>) => (
	<div
		role="img"
		aria-label={label}
		className="relative mx-auto w-full max-w-70"
	>
		<PieChart
			responsive
			accessibilityLayer={false}
			margin={{ top: 0, right: 0, bottom: 0, left: 0 }}
			style={{ width: "100%", aspectRatio: "25 / 14.5" }}
		>
			<GaugeArcs value={value} zone={zone} />
		</PieChart>
		<GaugeNumber valueText={valueText} outOfText={outOfText} />
	</div>
);

export default IndexGauge;
