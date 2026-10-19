import { CaretLeftIcon } from "@phosphor-icons/react/dist/ssr";
import { PAGE_URLS } from "@shared/constants";
import { getI18n, getLanguageUrl } from "@shared/helpers";
import Link from "next/link";
import type { ReactNode } from "react";
import { ONBOARDING_STAGES } from "../constants/onboarding-stages";
import { ONBOARDING_STEPS } from "../constants/onboarding-steps";
import { FLOW_SHELL_I18N } from "../i18n/flow-shell-i18n";

interface FlowShellProps {
	language: string;
	step: number;
	children: ReactNode;
}

/** Frame of the onboarding: progress and the back link. */
const FlowShell = ({ language, step, children }: Readonly<FlowShellProps>) => {
	const t = getI18n(language, FLOW_SHELL_I18N);
	const totalSteps = ONBOARDING_STEPS.length;
	const stage = Object.values(ONBOARDING_STAGES).find((steps) =>
		steps.includes(step),
	);
	const backHref = stage?.includes(step - 1)
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
							<CaretLeftIcon size={22} />
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
