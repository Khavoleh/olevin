import { Card, Heading } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { Suspense } from "react";
import { ADVICE_CARD_I18N } from "../../i18n/advice-card-i18n";
import AdviceList from "./AdviceList";

interface AdviceCardProps {
	language: string;
	className?: string;
}

const SKELETON_ROWS = [1, 2, 3];

/** Card of the advices that raise the index most, or why there are none. */
const AdviceCard = ({
	language,
	className = "",
}: Readonly<AdviceCardProps>) => {
	const t = getI18n(language, ADVICE_CARD_I18N);

	return (
		<Card className={`grid content-start gap-3 ${className}`}>
			<Heading level={2}>{t("title")}</Heading>
			<div aria-live="polite">
				<Suspense
					fallback={
						<div aria-hidden className="grid gap-3">
							{SKELETON_ROWS.map((row) => (
								<div key={row} className="h-16 rounded-lg bg-skeleton" />
							))}
						</div>
					}
				>
					<AdviceList language={language} />
				</Suspense>
			</div>
		</Card>
	);
};

export default AdviceCard;
