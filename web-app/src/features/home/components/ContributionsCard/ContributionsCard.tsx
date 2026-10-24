"use client";

import { Card, Heading, showErrorToast, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { useEffect, useState } from "react";
import { getLatestContributions } from "../../actions/get-latest-contributions";
import type { GetContributionsResponse } from "../../api/home.schemas";
import { describeContributions } from "../../helpers/describe-contributions/describe-contributions";
import { formatIndex } from "../../helpers/format-index/format-index";
import { formatSinceMonth } from "../../helpers/format-since-month/format-since-month";
import { useHomeStore } from "../../home-store";
import { CONTRIBUTIONS_CARD_I18N } from "../../i18n/contributions-card-i18n";
import ContributionRow from "./ContributionRow";

interface ContributionsCardProps {
	language: string;
	className?: string;
}

const SKELETON_ROWS = [1, 2, 3, 4, 5];

const DELTA_CLASSES: Record<number, string> = {
	1: "text-positive",
	[-1]: "text-negative",
	0: "text-fg-muted",
};

/** Card of what changed the index since the previous snapshot, factor by factor. */
const ContributionsCard = ({
	language,
	className = "",
}: Readonly<ContributionsCardProps>) => {
	const t = getI18n(language, CONTRIBUTIONS_CARD_I18N);
	const index = useHomeStore((state) => state.index);
	const change = index?.change ?? null;
	const hasChange = change !== null;
	const [contributions, setContributions] = useState<
		GetContributionsResponse | null | undefined
	>(undefined);

	useEffect(() => {
		if (!hasChange) {
			return;
		}

		void getLatestContributions(language).then((result) => {
			if ("error" in result) {
				showErrorToast(result.error);
			} else {
				setContributions(result.contributions);
			}
		});
	}, [hasChange, language]);

	let body = (
		<div aria-hidden className="grid gap-3">
			{SKELETON_ROWS.map((row) => (
				<div key={row} className="h-8 rounded-lg bg-skeleton" />
			))}
		</div>
	);

	if ((index && !change) || contributions === null) {
		body = (
			<div className="rounded-lg bg-surface-sunken p-3">
				<Text variant="hint">{t("empty")}</Text>
			</div>
		);
	} else if (contributions) {
		const lines = describeContributions(contributions, t, language);
		const scale = Math.max(...lines.map((line) => Math.abs(line.points)));

		body = (
			<ul className="grid gap-2">
				{lines.map((line) => (
					<li key={line.key}>
						<ContributionRow
							label={line.label}
							detail={line.detail}
							points={line.points}
							scale={scale}
							language={language}
						/>
					</li>
				))}
			</ul>
		);
	}

	return (
		<Card className={`grid content-start gap-3 ${className}`}>
			<div className="flex flex-wrap items-baseline justify-between gap-x-2.5 gap-y-1">
				<Heading level={2}>
					{change
						? `${t("title")} ${t("since")} ${formatSinceMonth(change.year, change.month, language)}`
						: t("title")}
				</Heading>
				{index && change ? (
					<span
						className={`font-semibold text-sm tabular-nums ${DELTA_CLASSES[Math.sign(change.delta)]}`}
					>
						{formatIndex(index.value - change.delta, language)}{" "}
						<span aria-hidden>→</span>
						<span className="sr-only">{t("to")}</span>{" "}
						{formatIndex(index.value, language)}
					</span>
				) : null}
			</div>
			<div aria-live="polite">{body}</div>
		</Card>
	);
};

export default ContributionsCard;
