"use client";

import { formatMoney, formatSnapshotMonth, getI18n } from "@shared/helpers";
import { GridListItem } from "react-aria-components";
import type { EstimateResponse } from "../../api/snapshots.schemas";
import { SNAPSHOTS_I18N } from "../../snapshots-i18n";
import EstimateTag from "../EstimateTag";
import CardActions from "./CardActions";

interface EstimateCardProps {
	estimate: EstimateResponse;
	currency: string;
	language: string;
}

/** Dashed card of an estimate from onboarding: only its amounts, without an index; it can be changed, not deleted. */
const EstimateCard = ({
	estimate,
	currency,
	language,
}: Readonly<EstimateCardProps>) => {
	const t = getI18n(language, SNAPSHOTS_I18N);
	const month = formatSnapshotMonth(estimate.year, estimate.month, language);

	return (
		<GridListItem
			id={estimate.id}
			textValue={month}
			className="rounded-lg border border-input border-dashed outline-none data-focus-visible:ring-2 data-focus-visible:ring-ring"
		>
			<div className="grid grid-cols-[minmax(0,1fr)_auto] items-center gap-2 p-3.5">
				<div className="grid min-w-0 gap-1">
					<EstimateTag language={language} />
					<p className="font-semibold capitalize">{month}</p>
					<p className="text-fg-muted text-sm tabular-nums">
						<span className="whitespace-nowrap">
							{t("expenses")}{" "}
							{formatMoney(estimate.expenses, currency, language)}
						</span>{" "}
						·{" "}
						<span className="whitespace-nowrap">
							{t("saved").toLocaleLowerCase(language)}{" "}
							{formatMoney(estimate.saved, currency, language)}
						</span>
					</p>
				</div>
				<span className="text-fg-muted text-sm">{t("no_index")}</span>
			</div>
			<CardActions
				year={estimate.year}
				month={estimate.month}
				monthName={month}
				language={language}
				canDelete={false}
			/>
		</GridListItem>
	);
};

export default EstimateCard;
