import type { ReactNode } from "react";

interface CardProps {
	children: ReactNode;
	className?: string;
}

/** White rounded panel that fills the width it is given. */
const Card = ({ children, className = "" }: Readonly<CardProps>) => {
	return (
		<section
			className={`w-full rounded-xl border border-border bg-surface p-4.5 shadow-card lg:p-5.5 ${className}`}
		>
			{children}
		</section>
	);
};

export default Card;
