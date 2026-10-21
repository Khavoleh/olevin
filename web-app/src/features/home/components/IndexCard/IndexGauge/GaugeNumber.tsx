interface GaugeNumberProps {
	valueText: string;
	outOfText: string;
}

/** Index number under the arc of the gauge. */
const GaugeNumber = ({ valueText, outOfText }: Readonly<GaugeNumberProps>) => (
	<div
		aria-hidden
		className="absolute inset-x-0 bottom-0.5 grid justify-items-center"
	>
		<span className="font-semibold text-display tabular-nums">{valueText}</span>
		<span className="text-fg-muted text-xs">{outOfText}</span>
	</div>
);

export default GaugeNumber;
