import { formatPoints } from "../../helpers/format-points/format-points";
import ContributionBar from "./ContributionBar";

interface ContributionRowProps {
	label: string;
	detail?: string;
	points: number;
	scale: number;
	language: string;
}

/** One line of the change: the factor with its values, the bar and the points. */
const ContributionRow = ({
	label,
	detail,
	points,
	scale,
	language,
}: Readonly<ContributionRowProps>) => {
	return (
		<div className="grid grid-cols-[minmax(0,1.4fr)_minmax(0,1fr)_2.75rem] items-center gap-2 text-sm">
			<div className="min-w-0">
				<span className="block">{label}</span>
				{detail ? (
					<span className="block text-fg-muted text-xs tabular-nums">
						{detail}
					</span>
				) : null}
			</div>
			<ContributionBar points={points} scale={scale} />
			<span className="text-right font-semibold tabular-nums">
				{formatPoints(points, language)}
			</span>
		</div>
	);
};

export default ContributionRow;
