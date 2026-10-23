"use client";

import { Card, Heading, showErrorToast } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { useEffect, useState } from "react";
import { getLatestComposition } from "../../actions/get-latest-composition";
import type { GetCompositionResponse } from "../../api/home.schemas";
import { describeBuffer } from "../../helpers/describe-buffer/describe-buffer";
import { describeCashFlow } from "../../helpers/describe-cash-flow/describe-cash-flow";
import { COMPOSITION_CARD_I18N } from "../../i18n/composition-card-i18n";
import SavingsRegularity from "./SavingsRegularity";
import SubsystemMeter from "./SubsystemMeter";

interface CompositionCardProps {
	language: string;
	className?: string;
}

const SKELETON_ITEMS = [1, 2, 3];

/** Card of what the index is made of: cash flow, financial buffer and savings regularity. */
const CompositionCard = ({
	language,
	className = "",
}: Readonly<CompositionCardProps>) => {
	const t = getI18n(language, COMPOSITION_CARD_I18N);
	const [composition, setComposition] = useState<GetCompositionResponse | null>(
		null,
	);

	useEffect(() => {
		void getLatestComposition(language).then((result) => {
			if ("error" in result) {
				showErrorToast(result.error);
			} else {
				setComposition(result.composition);
			}
		});
	}, [language]);

	return (
		<Card className={`grid content-start gap-3 ${className}`}>
			<Heading level={2}>{t("title")}</Heading>
			<div aria-live="polite" className="grid gap-4">
				{composition ? (
					<>
						<SubsystemMeter
							title={t("cash_flow")}
							value={composition.cashFlow.value}
							term={composition.cashFlow.term}
							detail={describeCashFlow(composition.cashFlow, t, language)}
							language={language}
						/>
						<SubsystemMeter
							title={t("buffer")}
							value={composition.buffer.value}
							term={composition.buffer.term}
							detail={describeBuffer(composition.buffer, t, language)}
							language={language}
						/>
						<SavingsRegularity
							regularity={composition.savingsRegularity}
							language={language}
						/>
					</>
				) : (
					SKELETON_ITEMS.map((item) => (
						<div
							key={item}
							aria-hidden
							className="h-20 rounded-lg bg-skeleton"
						/>
					))
				)}
			</div>
		</Card>
	);
};

export default CompositionCard;
