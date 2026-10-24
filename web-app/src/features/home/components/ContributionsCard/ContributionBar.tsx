interface ContributionBarProps {
	points: number;
	scale: number;
}

const DIRECTION_CLASSES: Record<number, string> = {
	1: "left-1/2 bg-positive",
	[-1]: "right-1/2 bg-negative",
	0: "",
};

/** Bar from the middle: right for a rise, left for a fall, the largest change reaching the edge. */
const ContributionBar = ({ points, scale }: Readonly<ContributionBarProps>) => {
	const width = scale > 0 ? (Math.abs(points) / scale) * 50 : 0;

	return (
		<div aria-hidden className="relative h-2.5">
			<span className="absolute -inset-y-0.75 left-1/2 w-px bg-input" />
			{width > 0 ? (
				<span
					className={`absolute top-0 h-full rounded-sm ${DIRECTION_CLASSES[Math.sign(points)]}`}
					style={{ width: `${width}%` }}
				/>
			) : null}
		</div>
	);
};

export default ContributionBar;
