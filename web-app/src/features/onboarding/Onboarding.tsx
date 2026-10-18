import { getI18n } from "@shared/helpers";
import { ONBOARDING_STEPS } from "./constants/onboarding-steps";
import { ONBOARDING_I18N } from "./i18n/onboarding-i18n";

interface OnboardingProps {
	language: string;
	step: number;
}

/** Steps with a note under their name. */
const STEPS_WITH_NOTE = [6, 7];

/** Onboarding step with the list of steps. */
const Onboarding = ({ language, step }: Readonly<OnboardingProps>) => {
	const t = getI18n(language, ONBOARDING_I18N);

	return (
		<div className="mx-auto grid max-w-[61.25rem] gap-5 md:grid-cols-[13.75rem_minmax(0,1fr)] md:gap-10">
			<ol
				aria-label={t("steps")}
				className="sticky top-0 hidden content-start gap-0.5 md:grid"
			>
				{ONBOARDING_STEPS.map((number) => {
					const isCurrent = number === step;
					const isDone = number < step;

					return (
						<li
							key={number}
							aria-current={isCurrent ? "step" : undefined}
							className={`flex items-center gap-2.5 rounded-md border px-2.5 py-2 text-sm ${
								isCurrent
									? "border-border bg-surface font-semibold text-fg"
									: "border-transparent text-fg-muted"
							}`}
						>
							<span
								aria-hidden="true"
								className={`flex size-[22px] flex-none items-center justify-center rounded-full border-[1.5px] font-bold text-xs ${
									isDone
										? "border-primary bg-primary text-on-primary"
										: isCurrent
											? "border-primary text-primary"
											: "border-input"
								}`}
							>
								{number}
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
					step === 6 ? "max-w-flow-wide" : "max-w-flow"
				}`}
			>
				<h1>{t(`step_${step}`)}</h1>
			</section>
		</div>
	);
};

export default Onboarding;
