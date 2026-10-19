import type { ReactNode } from "react";

interface CardProps {
	children: ReactNode;
	className?: string;
}

/** White rounded panel that holds the content of a page. */
const Card = ({ children, className = "" }: Readonly<CardProps>) => {
	return (
		<section
			className={`w-full max-w-md rounded-xl border border-border bg-surface p-4.5 shadow-card lg:p-5.5 ${className}`}
		>
			{children}
		</section>
	);
};

export default Card;
