import { MONTH_AXIS_HEIGHT, PLOT_TOP } from "../../constants/chart-layout";

interface ScaleLabelsProps {
	labels: { value: number; label: string }[];
	className?: string;
	itemClassName?: string;
}

/** Labels of the 0–100 scale beside the plot, at the heights of their values. */
const ScaleLabels = ({
	labels,
	className = "",
	itemClassName = "",
}: Readonly<ScaleLabelsProps>) => (
	<div
		aria-hidden
		className={`relative h-64 shrink-0 font-medium text-chart-axis md:h-76 ${className}`}
	>
		{labels.map((item) => (
			<span
				key={item.value}
				className={`absolute -translate-y-1/2 whitespace-nowrap ${itemClassName}`}
				style={{
					top: `calc(${PLOT_TOP}px + (100% - ${PLOT_TOP + MONTH_AXIS_HEIGHT}px) * ${(100 - item.value) / 100})`,
				}}
			>
				{item.label}
			</span>
		))}
	</div>
);

export default ScaleLabels;
