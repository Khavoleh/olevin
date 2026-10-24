import { Card, Heading } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { Suspense } from "react";
import { CONTRIBUTIONS_CARD_I18N } from "../../i18n/contributions-card-i18n";
import ContributionsHeading from "./ContributionsHeading";
import ContributionsList from "./ContributionsList";

interface ContributionsCardProps {
	language: string;
	className?: string;
}

const SKELETON_ROWS = [1, 2, 3, 4, 5];

/** Card of what changed the index since the previous snapshot, factor by factor. */
const ContributionsCard = ({
	language,
	className = "",
}: Readonly<ContributionsCardProps>) => {
	const t = getI18n(language, CONTRIBUTIONS_CARD_I18N);

	return (
		<Card className={`grid content-start gap-3 ${className}`}>
			<div className="flex flex-wrap items-baseline justify-between gap-x-2.5 gap-y-1">
				<Suspense fallback={<Heading level={2}>{t("title")}</Heading>}>
					<ContributionsHeading language={language} />
				</Suspense>
			</div>
			<div aria-live="polite">
				<Suspense
					fallback={
						<div aria-hidden className="grid gap-3">
							{SKELETON_ROWS.map((row) => (
								<div key={row} className="h-8 rounded-lg bg-skeleton" />
							))}
						</div>
					}
				>
					<ContributionsList language={language} />
				</Suspense>
			</div>
		</Card>
	);
};

export default ContributionsCard;
