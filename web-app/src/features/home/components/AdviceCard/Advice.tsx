import type { AdviceResponse } from "../../api/home.schemas";
import DebtToIncomeAdvice from "./DebtToIncomeAdvice";
import ExpenseVariationAdvice from "./ExpenseVariationAdvice";
import ReserveAdvice from "./ReserveAdvice";
import SavingsRateAdvice from "./SavingsRateAdvice";
import SavingsRegularityAdvice from "./SavingsRegularityAdvice";

interface AdviceProps {
	advice: AdviceResponse;
	rank: number;
	currency: string;
	language: string;
}

/** Advice of the indicator named by its `kind`. */
const Advice = ({
	advice,
	rank,
	currency,
	language,
}: Readonly<AdviceProps>) => {
	switch (advice.kind) {
		case "SavingsRate":
			return (
				<SavingsRateAdvice
					advice={advice}
					rank={rank}
					currency={currency}
					language={language}
				/>
			);
		case "Reserve":
			return (
				<ReserveAdvice
					advice={advice}
					rank={rank}
					currency={currency}
					language={language}
				/>
			);
		case "DebtToIncome":
			return (
				<DebtToIncomeAdvice
					advice={advice}
					rank={rank}
					currency={currency}
					language={language}
				/>
			);
		case "ExpenseVariation":
			return (
				<ExpenseVariationAdvice
					advice={advice}
					rank={rank}
					currency={currency}
					language={language}
				/>
			);
		case "SavingsRegularity":
			return (
				<SavingsRegularityAdvice
					advice={advice}
					rank={rank}
					language={language}
				/>
			);
	}
};

export default Advice;
