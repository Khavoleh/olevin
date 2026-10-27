"use client";

import { getFormProps, useForm } from "@conform-to/react";
import { parseWithZod } from "@conform-to/zod/v4";
import { InfoIcon } from "@phosphor-icons/react";
import { Text, useFormErrorToast } from "@shared/components";
import { PAGE_URLS } from "@shared/constants";
import { getI18n, getLanguageUrl } from "@shared/helpers";
import { EstimateFields, HiddenFields } from "@widgets/snapshot-fields";
import { useRouter } from "next/navigation";
import { useActionState, useEffect } from "react";
import { saveFirstSnapshot } from "../actions/save-first-snapshot";
import { CreateFirstSnapshotBody } from "../api/onboarding.zod";
import { getFirstSnapshotRequest } from "../helpers/get-first-snapshot-request";
import { ESTIMATES_STEP_I18N } from "../i18n/estimates-step-i18n";
import { useOnboardingStore } from "../onboarding-store";
import { useSnapshotMonths } from "../use-snapshot-months";
import SubmitButton from "./SubmitButton";

type EstimatedMonth = "previousMonth" | "twoMonthsAgo";

interface EstimatesStepProps {
	language: string;
}

/** Step with the rough expenses and savings of the 2 months before the first snapshot that sends the whole snapshot. */
const EstimatesStep = ({ language }: Readonly<EstimatesStepProps>) => {
	const t = getI18n(language, ESTIMATES_STEP_I18N);
	const router = useRouter();
	const baseCurrency = useOnboardingStore((state) => state.baseCurrency);
	const snapshot = useOnboardingStore((state) => state.snapshot);
	const setSnapshot = useOnboardingStore((state) => state.setSnapshot);
	const [, previous, twoMonthsAgo] = useSnapshotMonths(language);
	const [lastResult, action, isPending] = useActionState(
		saveFirstSnapshot,
		undefined,
	);
	const [form] = useForm({
		lastResult,
		onValidate: ({ formData }) =>
			parseWithZod(formData, { schema: CreateFirstSnapshotBody }),
	});
	const request = baseCurrency
		? getFirstSnapshotRequest(snapshot, baseCurrency)
		: undefined;
	const isComplete = CreateFirstSnapshotBody.safeParse(request).success;
	const months = [
		{ key: "previousMonth", title: previous?.title },
		{ key: "twoMonthsAgo", title: twoMonthsAgo?.title },
	] satisfies { key: EstimatedMonth; title?: string }[];

	useFormErrorToast(lastResult);

	useEffect(() => {
		void useOnboardingStore.persist.rehydrate()?.then(() => {
			if (!useOnboardingStore.getState().snapshot.selfRating) {
				router.replace(getLanguageUrl(`${PAGE_URLS.ONBOARDING}/2`, language));
			}
		});
	}, [language, router]);

	return (
		<form {...getFormProps(form)} action={action} className="grid gap-4">
			<Text variant="description">
				{previous &&
					twoMonthsAgo &&
					t("description")
						.replace("{previous}", previous.name)
						.replace("{two_months_ago}", twoMonthsAgo.name)}
			</Text>

			<input type="hidden" name="language" value={language} />
			{request && <HiddenFields value={request} />}

			{months.map(({ key, title }) => (
				<EstimateFields
					key={key}
					language={language}
					title={title ?? ""}
					value={snapshot[key]}
					baseCurrency={baseCurrency}
					onChange={(estimate) => setSnapshot({ [key]: estimate })}
				/>
			))}

			<div className="flex gap-2.5 rounded-lg bg-primary-soft p-3">
				<InfoIcon size={20} className="mt-0.5 flex-none text-primary" />
				<Text>{t("note")}</Text>
			</div>

			<SubmitButton
				language={language}
				isComplete={isComplete}
				isPending={isPending}
			>
				{t("continue")}
			</SubmitButton>
		</form>
	);
};

export default EstimatesStep;
