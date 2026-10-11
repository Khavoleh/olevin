"use client";

import { getPathLanguage } from "@shared/helpers";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { type ReactNode, Suspense } from "react";
import { getActiveNavItem } from "./helpers/get-active-nav-item/get-active-nav-item";
import type { NavItemId } from "./nav-item-id";

interface AppNavLinkProps {
	id: NavItemId;
	href: string;
	className: string;
	children: ReactNode;
	"aria-label"?: string;
}

/** Link that knows the current page, so it can mark itself with `aria-current`. */
const ActiveNavLink = ({
	id,
	href,
	className,
	children,
	...props
}: Readonly<AppNavLinkProps>) => {
	const pathname = usePathname();
	const language = getPathLanguage(pathname);
	const page = language ? pathname.slice(language.length + 1) : pathname;
	const isActive = getActiveNavItem(page) === id;

	return (
		<Link
			href={href}
			className={className}
			aria-current={isActive ? "page" : undefined}
			{...props}
		>
			{children}
		</Link>
	);
};

/**
 * Navigation link that marks itself with `aria-current` on its own section; the styles read that attribute.
 * The pathname is request data on dynamic routes, so until it arrives the link is a plain one.
 */
const AppNavLink = ({ id, children, ...props }: Readonly<AppNavLinkProps>) => {
	return (
		<Suspense
			fallback={
				<Link
					href={props.href}
					className={props.className}
					aria-label={props["aria-label"]}
				>
					{children}
				</Link>
			}
		>
			<ActiveNavLink id={id} {...props}>
				{children}
			</ActiveNavLink>
		</Suspense>
	);
};

export default AppNavLink;
