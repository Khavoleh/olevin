import type { LabelProps } from "recharts";

interface ValueLabelProps {
	viewBox: LabelProps["viewBox"];
	text?: string;
	isLast: boolean;
}

/** Value above a dot; the latest one is bold. */
const ValueLabel = ({ viewBox, text, isLast }: Readonly<ValueLabelProps>) =>
	!text || !viewBox || !("x" in viewBox) ? (
		<g />
	) : (
		<text
			x={viewBox.x}
			y={(viewBox.y ?? 0) - 12}
			textAnchor="middle"
			fill="var(--color-fg)"
			fontSize={13}
			fontWeight={isLast ? 700 : 500}
			className="tabular-nums"
		>
			{text}
		</text>
	);

export default ValueLabel;
