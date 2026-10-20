import { CheckIcon } from "@phosphor-icons/react/dist/ssr";
import { Heading, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import EstimatesStep from "./components/EstimatesStep";
import RestoreBaseCurrency from "./components/RestoreBaseCurrency";
import SettingsStep from "./components/SettingsStep/SettingsStep";
import SnapshotStep from "./components/SnapshotStep/SnapshotStep";
import { ONBOARDING_STAGES } from "./constants/onboarding-stages";
import { ONBOARDING_STEPS } from "./constants/onboarding-steps";
import { ONBOARDING_I18N } from "./i18n/onboarding-i18n";

interface OnboardingProps {
	language: string;
	step: number;
}

/** Steps with a note under their name. */
const STEPS_WITH_NOTE = [4];

/** Steps with a wider form. */
const WIDE_STEPS = [2, 4];

/** Classes of an item in the list of steps and of its number, by the state of the step. */
const STEP_CLASSES = {
	done: {
		item: "border-transparent text-fg-muted",
		number: "border-primary bg-primary text-on-primary",
	},
	current: {
		item: "border-border bg-surface font-semibold text-fg",
		number: "border-primary text-primary",
	},
	upcoming: {
		item: "border-transparent text-fg-muted",
		number: "border-input",
	},
};

/** Onboarding step with the list of steps. */
const Onboarding = ({ language, step }: Readonly<OnboardingProps>) => {
	const t = getI18n(language, ONBOARDING_I18N);
	const stepOf = t("step_of")
		.replace("{step}", String(step))
		.replace("{total}", String(ONBOARDING_STEPS.length));

	return (
		<div className="mx-auto grid max-w-245 gap-5 md:grid-cols-[13.75rem_minmax(0,1fr)] md:gap-10">
			<ol
				aria-label={t("steps")}
				className="sticky top-0 hidden content-start gap-0.5 md:grid"
			>
				{ONBOARDING_STEPS.map((number) => {
					const state =
						(number < step && "done") ||
						(number === step && "current") ||
						"upcoming";

					return (
						<li
							key={number}
							aria-current={state === "current" ? "step" : undefined}
							className={`flex items-center gap-2.5 rounded-md border px-2.5 py-2 text-sm ${STEP_CLASSES[state].item}`}
						>
							<span
								aria-hidden="true"
								className={`flex size-5.5 flex-none items-center justify-center rounded-full border-[1.5px] font-bold text-xs ${STEP_CLASSES[state].number}`}
							>
								{state === "done" ? (
									<CheckIcon size={13} weight="bold" />
								) : (
									number
								)}
							</span>
							<span>
								{t(`step_${number}`)}
								{STEPS_WITH_NOTE.includes(number) && (
									<small className="block font-normal text-xs">
										{t(`step_${number}_note`)}
									</small>
								)}
							</span>
						</li>
					);
				})}
			</ol>

			<section
				className={`grid min-w-0 content-start gap-4 ${
					WIDE_STEPS.includes(step) ? "max-w-flow-wide" : "max-w-flow"
				}`}
			>
				<div className="grid gap-1">
					<Text variant="eyebrow">{stepOf}</Text>
					<Heading level={1}>{t(`step_${step}`)}</Heading>
				</div>
				{step === 1 && <SettingsStep language={language} />}
				{step === 2 && <SnapshotStep language={language} />}
				{step === 3 && <EstimatesStep language={language} />}
				{ONBOARDING_STAGES.Onboarding.includes(step) && (
					<RestoreBaseCurrency language={language} />
				)}
			</section>
		</div>
	);
};

export default Onboarding;
