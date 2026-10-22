"use client";

import { Card, Heading, showErrorToast, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { useEffect, useState } from "react";
import { getLatestAdvices } from "../../actions/get-latest-advices";
import type { GetAdvicesResponse } from "../../api/home.schemas";
import { ADVICE_CARD_I18N } from "../../i18n/advice-card-i18n";
import Advice from "./Advice";

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
	const [advices, setAdvices] = useState<GetAdvicesResponse | null>(null);

	useEffect(() => {
		void getLatestAdvices(language).then((result) => {
			if ("error" in result) {
				showErrorToast(result.error);
			} else {
				setAdvices(result.advices);
			}
		});
	}, [language]);

	let body = (
		<div aria-hidden className="grid gap-3">
			{SKELETON_ROWS.map((row) => (
				<div key={row} className="h-16 rounded-lg bg-skeleton" />
			))}
		</div>
	);

	if (advices?.status === "Shown") {
		body = (
			<ol className="grid gap-3">
				{advices.advices.map((advice, index) => (
					<Advice
						key={advice.kind}
						advice={advice}
						rank={index + 1}
						currency={advices.currency}
						language={language}
					/>
				))}
			</ol>
		);
	} else if (advices) {
		body = <Text variant="description">{t(`status_${advices.status}`)}</Text>;
	}

	return (
		<Card className={`grid content-start gap-3 ${className}`}>
			<Heading level={2}>{t("title")}</Heading>
			<div aria-live="polite">{body}</div>
		</Card>
	);
};

export default AdviceCard;
