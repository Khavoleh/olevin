import { Card, Heading, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { Suspense } from "react";
import { INDEX_CARD_I18N } from "../../i18n/index-card-i18n";
import IndexSummary from "./IndexSummary";

interface IndexCardProps {
	language: string;
	className?: string;
}

/** Card of the latest index: the gauge, its zone and the change since the previous snapshot. */
const IndexCard = ({ language, className = "" }: Readonly<IndexCardProps>) => {
	const t = getI18n(language, INDEX_CARD_I18N);

	return (
		<Card className={`grid content-start gap-3 ${className}`}>
			<div className="flex items-center justify-between gap-2">
				<Heading level={2}>{t("title")}</Heading>
				<Text variant="hint">{t("scale")}</Text>
			</div>

			<Suspense
				fallback={
					<>
						<div
							aria-hidden
							className="mx-auto aspect-25/14 w-full max-w-70 rounded-lg bg-skeleton"
						/>
						<div className="flex flex-wrap items-center justify-center gap-2.5">
							<span
								aria-hidden
								className="h-6.5 w-40 rounded-full bg-skeleton"
							/>
						</div>
					</>
				}
			>
				<IndexSummary language={language} />
			</Suspense>

			<div className="flex justify-between gap-1.5 border-border border-t pt-2.5 text-fg-muted text-xs tabular-nums">
				<span>{t("legend_stress")}</span>
				<span>{t("legend_unstable")}</span>
				<span>{t("legend_calm")}</span>
			</div>
		</Card>
	);
};

export default IndexCard;
