import { Card } from "@shared/components";
import { Suspense } from "react";
import AccountSummaryCard from "./AccountSummaryCard";

interface AccountSummaryProps {
	language: string;
}

/** Card of the signed-in account with its skeleton while the settings load. */
const AccountSummary = ({ language }: Readonly<AccountSummaryProps>) => (
	<Suspense
		fallback={
			<Card className="flex items-center gap-3.5">
				<span
					aria-hidden
					className="size-13 flex-none rounded-full bg-skeleton"
				/>
				<span aria-hidden className="grid gap-1.5">
					<span className="h-5 w-44 rounded-md bg-skeleton" />
					<span className="h-4 w-36 rounded-md bg-skeleton" />
				</span>
			</Card>
		}
	>
		<AccountSummaryCard language={language} />
	</Suspense>
);

export default AccountSummary;
