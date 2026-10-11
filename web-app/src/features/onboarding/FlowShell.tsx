import { CaretLeft } from "@phosphor-icons/react/dist/ssr";
import { PAGE_URLS } from "@shared/constants";
import { getI18n, getLanguageUrl } from "@shared/helpers";
import Link from "next/link";
import type { ReactNode } from "react";
import { FLOW_SHELL_I18N } from "./flow-shell-i18n";
import { ONBOARDING_STEPS } from "./onboarding-steps";

interface FlowShellProps {
	language: string;
	/** Number of the current step, starting from 1. */
	step: number;
	children: ReactNode;
}

/** Frame of the onboarding: progress bar and the back link on top, one step below, no app navigation. */
const FlowShell = ({ language, step, children }: Readonly<FlowShellProps>) => {
	const t = getI18n(language, FLOW_SHELL_I18N);
	const totalSteps = ONBOARDING_STEPS.length;
	const backHref =
		step > 1
			? getLanguageUrl(`${PAGE_URLS.ONBOARDING}/${step - 1}`, language)
			: undefined;

	return (
		<div className="flex h-dvh flex-col">
			<header className="flex items-center gap-3 border-border border-b bg-surface px-4 py-2">
				<div className="size-11 flex-none">
					{backHref && (
						<Link
							href={backHref}
							aria-label={t("back")}
							className="flex size-11 items-center justify-center rounded-md text-fg hover:bg-surface-hover"
						>
							<CaretLeft size={22} />
						</Link>
					)}
				</div>
				<div
					role="progressbar"
					aria-label={t("progress")}
					aria-valuemin={0}
					aria-valuemax={totalSteps}
					aria-valuenow={step}
					className="h-1.5 flex-1 overflow-hidden rounded-full bg-meter-track"
				>
					<div
						className="h-full rounded-full bg-meter transition-[width] duration-300 ease-standard"
						style={{ width: `${(step / totalSteps) * 100}%` }}
					/>
				</div>
				<span className="flex-none text-fg-muted text-sm tabular-nums">
					{step} / {totalSteps}
				</span>
			</header>

			<main className="min-h-0 flex-1 overflow-y-auto px-4 pt-5 pb-[calc(env(safe-area-inset-bottom)+1.5rem)] md:p-8">
				{children}
			</main>
		</div>
	);
};

export default FlowShell;
