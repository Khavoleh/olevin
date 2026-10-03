import type { ReactNode } from "react";

interface CardProps {
	children: ReactNode;
	className?: string;
}

/** White rounded panel that holds the content of a page. */
const Card = ({ children, className = "" }: Readonly<CardProps>) => {
	return (
		<section
			className={`w-full max-w-md rounded-2xl border border-border bg-surface p-6 shadow-sm ${className}`}
		>
			{children}
		</section>
	);
};

export default Card;
