"use client";

import { Card, Heading, Text, ZoneBadge } from "@shared/components";
import { formatIndex, getI18n } from "@shared/helpers";
import { useHomeStore } from "../../home-store";
import { INDEX_CARD_I18N } from "../../i18n/index-card-i18n";
import IndexChange from "./IndexChange";
import IndexGauge from "./IndexGauge/IndexGauge";

interface IndexCardProps {
	language: string;
	className?: string;
}

/** Card of the latest index: the gauge, its zone and the change since the previous snapshot. */
const IndexCard = ({ language, className = "" }: Readonly<IndexCardProps>) => {
	const t = getI18n(language, INDEX_CARD_I18N);
	const index = useHomeStore((state) => state.index);

	return (
		<Card className={`grid content-start gap-3 ${className}`}>
			<div className="flex items-center justify-between gap-2">
				<Heading level={2}>{t("title")}</Heading>
				<Text variant="hint">{t("scale")}</Text>
			</div>

			{index ? (
				<IndexGauge
					value={index.value}
					zone={index.zone}
					valueText={formatIndex(index.value, language)}
					outOfText={t("out_of")}
					label={`${t("title")} ${formatIndex(index.value, language)} ${t("out_of")}`}
				/>
			) : (
				<div
					aria-hidden
					className="mx-auto aspect-25/14 w-full max-w-70 rounded-lg bg-skeleton"
				/>
			)}

			<div className="flex flex-wrap items-center justify-center gap-2.5">
				{index ? (
					<>
						<ZoneBadge zone={index.zone} language={language} />
						{index.change ? (
							<IndexChange change={index.change} language={language} />
						) : null}
					</>
				) : (
					<span aria-hidden className="h-6.5 w-40 rounded-full bg-skeleton" />
				)}
			</div>

			<div className="flex justify-between gap-1.5 border-border border-t pt-2.5 text-fg-muted text-xs tabular-nums">
				<span>{t("legend_stress")}</span>
				<span>{t("legend_unstable")}</span>
				<span>{t("legend_calm")}</span>
			</div>
		</Card>
	);
};

export default IndexCard;
