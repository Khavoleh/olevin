import {
	formatShortMonth,
	formatSnapshotMonth,
	getI18n,
} from "@shared/helpers";
import type { SavingsRegularityResponse } from "../../api/home.schemas";
import { COMPOSITION_CARD_I18N } from "../../i18n/composition-card-i18n";
import CompositionItem from "./CompositionItem";

interface SavingsRegularityProps {
	regularity: SavingsRegularityResponse;
	language: string;
}

const COLUMN_CLASSES: Record<number, string> = {
	1: "grid-cols-1",
	2: "grid-cols-2",
	3: "grid-cols-3",
	4: "grid-cols-4",
	5: "grid-cols-5",
	6: "grid-cols-6",
};

/** Savings regularity as a strip of recent months, the ones with savings filled. */
const SavingsRegularity = ({
	regularity,
	language,
}: Readonly<SavingsRegularityProps>) => {
	const t = getI18n(language, COMPOSITION_CARD_I18N);
	const savingMonths = regularity.months.filter(
		(month) => month.hasSaved,
	).length;

	return (
		<CompositionItem
			title={t("regularity")}
			summary={
				<>
					<span className="font-semibold text-fg">{savingMonths}</span>{" "}
					{t("of")} {regularity.months.length} {t("months")} ·{" "}
					{t(`regularity_${regularity.term}`)}
				</>
			}
		>
			<ol className={`grid gap-1 ${COLUMN_CLASSES[regularity.months.length]}`}>
				{regularity.months.map(({ year, month, hasSaved }) => (
					<li
						key={`${year}-${month}`}
						className="grid justify-items-center gap-0.75 text-fg-muted text-xs"
					>
						<span
							aria-hidden
							className={`h-2 w-full rounded-full ${hasSaved ? "bg-meter" : "bg-meter-track"}`}
						/>
						<span aria-hidden>{formatShortMonth(year, month, language)}</span>
						<span className="sr-only">
							{formatSnapshotMonth(year, month, language)}:{" "}
							{t(hasSaved ? "saved" : "not_saved")}
						</span>
					</li>
				))}
			</ol>
		</CompositionItem>
	);
};

export default SavingsRegularity;
