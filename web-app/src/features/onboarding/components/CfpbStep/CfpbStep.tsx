"use client";

import { getFormProps, useForm } from "@conform-to/react";
import { parseWithZod } from "@conform-to/zod/v4";
import { Button, Text, useFormErrorToast } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { useActionState, useEffect } from "react";
import { saveCfpbResponse } from "../../actions/save-cfpb-response";
import {
	CfpbDescription,
	CfpbFrequency,
	type SubmitCfpbResponseRequest,
} from "../../api/onboarding.schemas";
import { BLOCK_CLASSES } from "../../constants/block-classes";
import { CFPB_STEP_SCHEMA } from "../../constants/cfpb-step-schema";
import { CFPB_STEP_I18N } from "../../i18n/cfpb-step-i18n";
import { useOnboardingStore } from "../../onboarding-store";
import FormBlock from "../FormBlock";
import AnswerField from "./AnswerField";

type CfpbStatement = Exclude<keyof SubmitCfpbResponseRequest, "is62Plus">;

const STATEMENT_LABELS: Record<CfpbStatement, string> = {
	couldHandleMajorExpense: "could_handle_major_expense",
	securingFinancialFuture: "securing_financial_future",
	neverHaveThingsWanted: "never_have_things_wanted",
	enjoyLifeManagingMoney: "enjoy_life_managing_money",
	justGettingBy: "just_getting_by",
	moneyWontLast: "money_wont_last",
	giftStrainsFinances: "gift_strains_finances",
	moneyLeftOver: "money_left_over",
	behindWithFinances: "behind_with_finances",
	financesControlLife: "finances_control_life",
};

const OPTION_LABELS: Record<CfpbDescription | CfpbFrequency, string> = {
	Completely: "completely",
	VeryWell: "very_well",
	Somewhat: "somewhat",
	VeryLittle: "very_little",
	NotAtAll: "not_at_all",
	Always: "always",
	Often: "often",
	Sometimes: "sometimes",
	Rarely: "rarely",
	Never: "never",
};

/** Statements in their order, grouped by the scale they are answered on. */
const STATEMENT_BLOCKS = [
	{
		title: "describes",
		options: Object.values(CfpbDescription),
		statements: [
			"couldHandleMajorExpense",
			"securingFinancialFuture",
			"neverHaveThingsWanted",
			"enjoyLifeManagingMoney",
			"justGettingBy",
			"moneyWontLast",
		],
	},
	{
		title: "applies",
		options: Object.values(CfpbFrequency),
		statements: [
			"giftStrainsFinances",
			"moneyLeftOver",
			"behindWithFinances",
			"financesControlLife",
		],
	},
] satisfies {
	title: string;
	options: (CfpbDescription | CfpbFrequency)[];
	statements: CfpbStatement[];
}[];

const STATEMENTS: CfpbStatement[] = STATEMENT_BLOCKS.flatMap(
	({ statements }) => statements,
);

interface CfpbStepProps {
	language: string;
}

/** Step with the CFPB Financial Well-Being Scale for the users who consented to research, which finishes the onboarding. */
const CfpbStep = ({ language }: Readonly<CfpbStepProps>) => {
	const t = getI18n(language, CFPB_STEP_I18N);
	const [lastResult, action, isPending] = useActionState(
		saveCfpbResponse,
		undefined,
	);
	const [form, fields] = useForm({
		lastResult,
		onValidate: ({ formData }) =>
			parseWithZod(formData, { schema: CFPB_STEP_SCHEMA }),
		shouldRevalidate: "onInput",
	});

	useFormErrorToast(lastResult);

	useEffect(() => {
		void useOnboardingStore.persist.rehydrate();
	}, []);

	return (
		<form {...getFormProps(form)} action={action} className="grid gap-4">
			<Text variant="description">{t("description")}</Text>

			<input type="hidden" name="language" value={language} />

			{STATEMENT_BLOCKS.map(({ title, options, statements }) => (
				<FormBlock key={title} title={t(title)}>
					<div className="grid divide-y divide-border">
						{statements.map((statement) => (
							<AnswerField
								key={statement}
								answer={statement}
								label={`${STATEMENTS.indexOf(statement) + 1}. ${t(STATEMENT_LABELS[statement])}`}
								errorMessage={t("answer_error")}
								labelVariant="field"
								options={options.map((option) => ({
									value: option,
									label: t(OPTION_LABELS[option]),
								}))}
								isInvalid={!!fields[statement].errors}
								className="py-3 first:pt-1 last:pb-0"
								optionsClassName="@lg:grid-cols-5"
							/>
						))}
					</div>
				</FormBlock>
			))}

			<div className={BLOCK_CLASSES}>
				<AnswerField
					answer="is62Plus"
					label={t("age")}
					description={t("age_hint")}
					errorMessage={t("age_error")}
					options={[
						{ value: "false", label: t("under_62") },
						{ value: "true", label: t("from_62") },
					]}
					isInvalid={!!fields.is62Plus.errors}
					optionsClassName="grid-cols-2"
				/>
			</div>

			<Button
				type="submit"
				isPending={isPending}
				className="md:min-w-65 md:justify-self-start"
			>
				{t("finish")}
			</Button>
		</form>
	);
};

export default CfpbStep;
