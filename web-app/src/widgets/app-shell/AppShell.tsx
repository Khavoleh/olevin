import {
	HouseIcon,
	ListBulletsIcon,
	TrendUpIcon,
	UserIcon,
} from "@phosphor-icons/react/dist/ssr";
import { Logo } from "@shared/components";
import { PAGE_URLS } from "@shared/constants";
import { getI18n, getLanguageUrl } from "@shared/helpers";
import LegalLinks from "@widgets/legal-links";
import Link from "next/link";
import type { ReactNode } from "react";
import AppNavLink from "./AppNavLink";
import { APP_SHELL_I18N } from "./app-shell-i18n";
import NewSnapshotNavItem from "./NewSnapshotNavItem";

interface AppShellProps {
	language: string;
	/** Day the next month opens for a snapshot, or `null` while a month is open. */
	newSnapshotOpensOn: Promise<string | null>;
	children: ReactNode;
}

/** Link of the narrow rail (tablet) and the sidebar (laptop); the sidebar puts the label next to the icon. */
const SIDE_ITEM_CLASSES =
	"flex min-h-11 flex-col items-center gap-1 rounded-md px-0.5 py-2 text-center font-medium text-fg-muted text-xs hover:bg-surface-hover aria-[current=page]:bg-primary-soft aria-[current=page]:font-semibold aria-[current=page]:text-primary lg:flex-row lg:gap-3 lg:px-3 lg:text-left lg:text-base";

/** Link of the tab bar on a phone. */
const TAB_ITEM_CLASSES =
	"flex min-h-11 flex-col items-center gap-0.5 py-1 font-medium text-fg-muted text-xs aria-[current=page]:font-semibold aria-[current=page]:text-primary";

/**
 * Frame of the signed-in pages: tab bar on a phone, narrow rail on a tablet and sidebar on a laptop.
 * The content scrolls inside the frame, so the bars stay in place.
 */
const AppShell = ({
	language,
	newSnapshotOpensOn,
	children,
}: Readonly<AppShellProps>) => {
	const t = getI18n(language, APP_SHELL_I18N);
	const url = (page: string) => getLanguageUrl(page, language);

	return (
		<div className="flex h-dvh flex-col md:flex-row">
			<aside className="hidden w-(--layout-rail) flex-none flex-col items-center gap-2 border-border border-r bg-surface px-2 py-4 md:flex lg:w-(--layout-sidebar) lg:items-stretch lg:px-4 lg:py-5">
				<Link
					href={url(PAGE_URLS.HOME)}
					aria-label={t("overview")}
					className="mb-2.5 flex items-center gap-2 text-primary lg:px-2"
				>
					<Logo size={26} />
					<span className="hidden font-heading font-semibold text-fg text-xl lg:inline">
						Olevin
					</span>
				</Link>

				<NewSnapshotNavItem
					language={language}
					opensOn={newSnapshotOpensOn}
					placement="side"
				/>

				<nav
					aria-label={t("main_navigation")}
					className="mt-2 grid w-full gap-1"
				>
					<AppNavLink
						id="overview"
						href={url(PAGE_URLS.HOME)}
						className={SIDE_ITEM_CLASSES}
					>
						<HouseIcon size={20} />
						<span>{t("overview")}</span>
					</AppNavLink>
					<AppNavLink
						id="trend"
						href={url(PAGE_URLS.TREND)}
						className={SIDE_ITEM_CLASSES}
					>
						<TrendUpIcon size={20} />
						<span>{t("trend")}</span>
					</AppNavLink>
					<AppNavLink
						id="history"
						href={url(PAGE_URLS.SNAPSHOTS)}
						className={SIDE_ITEM_CLASSES}
					>
						<ListBulletsIcon size={20} />
						<span>{t("history")}</span>
					</AppNavLink>
				</nav>

				<div className="mt-auto grid w-full gap-2">
					<AppNavLink
						id="profile"
						href={url(PAGE_URLS.PROFILE)}
						className={SIDE_ITEM_CLASSES}
					>
						<UserIcon size={20} />
						<span>{t("profile")}</span>
					</AppNavLink>
					<LegalLinks
						language={language}
						className="hidden gap-y-1.5 border-border border-t px-3 pt-3 lg:grid"
					/>
				</div>
			</aside>

			<main className="relative min-h-0 min-w-0 flex-1 overflow-y-auto px-4 pt-5 pb-6 md:px-8 md:pt-7 md:pb-8 lg:px-10 lg:pt-8 lg:pb-10">
				<div className="mx-auto grid w-full max-w-content gap-4">
					{children}
					<LegalLinks
						language={language}
						className="justify-center pt-3 lg:hidden"
					/>
				</div>
			</main>

			<nav
				aria-label={t("main_navigation")}
				className="grid grid-cols-5 items-end border-border border-t bg-surface px-1 pt-1.5 pb-[calc(env(safe-area-inset-bottom)+0.5rem)] md:hidden"
			>
				<AppNavLink
					id="overview"
					href={url(PAGE_URLS.HOME)}
					className={TAB_ITEM_CLASSES}
				>
					<HouseIcon size={22} />
					<span>{t("overview")}</span>
				</AppNavLink>
				<AppNavLink
					id="trend"
					href={url(PAGE_URLS.TREND)}
					className={TAB_ITEM_CLASSES}
				>
					<TrendUpIcon size={22} />
					<span>{t("trend")}</span>
				</AppNavLink>
				<NewSnapshotNavItem
					language={language}
					opensOn={newSnapshotOpensOn}
					placement="tab"
				/>
				<AppNavLink
					id="history"
					href={url(PAGE_URLS.SNAPSHOTS)}
					className={TAB_ITEM_CLASSES}
				>
					<ListBulletsIcon size={22} />
					<span>{t("history")}</span>
				</AppNavLink>
				<AppNavLink
					id="profile"
					href={url(PAGE_URLS.PROFILE)}
					className={TAB_ITEM_CLASSES}
				>
					<UserIcon size={22} />
					<span>{t("profile")}</span>
				</AppNavLink>
			</nav>
		</div>
	);
};

export default AppShell;
