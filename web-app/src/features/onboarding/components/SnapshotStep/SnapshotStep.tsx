"use client";

import { getFormProps, useForm } from "@conform-to/react";
import { parseWithZod } from "@conform-to/zod/v4";
import { Button, Text } from "@shared/components";
import { PAGE_URLS } from "@shared/constants";
import { getI18n, getLanguageUrl } from "@shared/helpers";
import MoneyField, { MoneyList } from "@widgets/money-field";
import { useRouter } from "next/navigation";
import { useEffect } from "react";
import { BLOCK_CLASSES } from "../../constants/block-classes";
import { SNAPSHOT_STEP_SCHEMA } from "../../constants/snapshot-step-schema";
import { getFirstSnapshotRequest } from "../../helpers/get-first-snapshot-request/get-first-snapshot-request";
import { SNAPSHOT_STEP_I18N } from "../../i18n/snapshot-step-i18n";
import { useOnboardingStore } from "../../onboarding-store";
import { useSnapshotMonths } from "../../use-snapshot-months";
import FormBlock from "../FormBlock";
import HiddenFields from "../HiddenFields";
import ExpensesFields from "./ExpensesFields";
import SelfRatingField from "./SelfRatingField";

interface SnapshotStepProps {
	language: string;
}

/** Step with the amounts of the current month that keeps them in the draft until the next step sends them. */
const SnapshotStep = ({ language }: Readonly<SnapshotStepProps>) => {
	const t = getI18n(language, SNAPSHOT_STEP_I18N);
	const router = useRouter();
	const baseCurrency = useOnboardingStore((state) => state.baseCurrency);
	const snapshot = useOnboardingStore((state) => state.snapshot);
	const setSnapshot = useOnboardingStore((state) => state.setSnapshot);
	const [month] = useSnapshotMonths(language);
	const [form, fields] = useForm({
		onValidate: ({ formData }) =>
			parseWithZod(formData, { schema: SNAPSHOT_STEP_SCHEMA }),
		onSubmit: (event) => {
			event.preventDefault();
			router.push(getLanguageUrl(`${PAGE_URLS.ONBOARDING}/3`, language));
		},
		shouldRevalidate: "onInput",
	});
	const request = baseCurrency
		? getFirstSnapshotRequest(snapshot, baseCurrency)
		: undefined;

	useEffect(() => {
		void useOnboardingStore.persist.rehydrate();
	}, []);

	return (
		<form {...getFormProps(form)} className="grid gap-4">
			<Text variant="description">
				{month && t("description").replace("{month}", month.name)}
			</Text>

			{request && (
				<HiddenFields
					value={{
						...request,
						selfRating: undefined,
						previousMonth: undefined,
						twoMonthsAgo: undefined,
					}}
				/>
			)}

			<FormBlock title={t("income")} hint={t("income_hint")}>
				<MoneyList
					language={language}
					items={snapshot.income}
					baseCurrency={baseCurrency}
					onChange={(income) => setSnapshot({ income })}
				/>
			</FormBlock>

			<ExpensesFields language={language} />

			<FormBlock title={t("saved")} hint={t("saved_hint")}>
				<div className="grid gap-2.5 lg:grid-cols-2">
					<MoneyField
						language={language}
						label={t("saved_label")}
						value={snapshot.saved}
						baseCurrency={baseCurrency}
						onChange={(saved) => setSnapshot({ saved })}
					/>
				</div>
			</FormBlock>

			<FormBlock title={t("reserves")} hint={t("reserves_hint")}>
				<MoneyList
					language={language}
					items={snapshot.reserves}
					baseCurrency={baseCurrency}
					onChange={(reserves) => setSnapshot({ reserves })}
				/>
			</FormBlock>

			<FormBlock title={t("debt_payments")} hint={t("debt_payments_hint")}>
				<MoneyList
					language={language}
					items={snapshot.debtPayments}
					baseCurrency={baseCurrency}
					onChange={(debtPayments) => setSnapshot({ debtPayments })}
				/>
			</FormBlock>

			<div className={BLOCK_CLASSES}>
				<SelfRatingField
					language={language}
					name={fields.selfRating.name}
					isInvalid={!!fields.selfRating.errors}
				/>
			</div>

			<Button
				type="submit"
				isDisabled={!request}
				className="md:min-w-65 md:justify-self-start"
			>
				{t("continue")}
			</Button>
		</form>
	);
};

export default SnapshotStep;
