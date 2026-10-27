"use client";

import { getFormProps, useForm } from "@conform-to/react";
import { parseWithZod } from "@conform-to/zod/v4";
import { Text } from "@shared/components";
import { PAGE_URLS } from "@shared/constants";
import { getI18n, getLanguageUrl } from "@shared/helpers";
import SnapshotFields, { HiddenFields } from "@widgets/snapshot-fields";
import { useRouter } from "next/navigation";
import { useEffect } from "react";
import { SNAPSHOT_STEP_SCHEMA } from "../constants/snapshot-step-schema";
import { getFirstSnapshotRequest } from "../helpers/get-first-snapshot-request";
import { SNAPSHOT_STEP_I18N } from "../i18n/snapshot-step-i18n";
import { useOnboardingStore } from "../onboarding-store";
import { useSnapshotMonths } from "../use-snapshot-months";
import SubmitButton from "./SubmitButton";

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
	const isComplete = SNAPSHOT_STEP_SCHEMA.safeParse(request).success;

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

			<SnapshotFields
				language={language}
				value={snapshot}
				baseCurrency={baseCurrency}
				selfRatingName={fields.selfRating.name}
				isSelfRatingInvalid={!!fields.selfRating.errors}
				onChange={setSnapshot}
			/>

			<SubmitButton language={language} isComplete={isComplete}>
				{t("continue")}
			</SubmitButton>
		</form>
	);
};

export default SnapshotStep;
