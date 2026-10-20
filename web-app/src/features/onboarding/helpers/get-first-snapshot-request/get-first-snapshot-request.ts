import type { MoneyDraft } from "@widgets/money-field";
import type {
	CurrencyCode,
	FirstSnapshot,
	Money,
} from "../../api/onboarding.schemas";
import type { EstimateDraft, SnapshotDraft } from "../../onboarding-store";

/** First snapshot request; the self-rating is missing until the user picks it. */
export type FirstSnapshotRequest = Omit<FirstSnapshot, "selfRating"> &
	Partial<Pick<FirstSnapshot, "selfRating">>;

/** Request of the first snapshot from the draft: empty amounts become 0, missing currencies the base one. */
export const getFirstSnapshotRequest = (
	draft: SnapshotDraft,
	baseCurrency: CurrencyCode,
): FirstSnapshotRequest => {
	const toMoney = ({ amount, currency }: MoneyDraft): Money => ({
		amount: amount ?? 0,
		currency: currency ?? baseCurrency,
	});
	const toEstimate = ({ expensesTotal, saved }: EstimateDraft) => ({
		expensesTotal: toMoney(expensesTotal),
		saved: toMoney(saved),
	});
	const { food, housing, health, clothing, restaurants, transport, other } =
		draft.expenses;

	return {
		income: draft.income.map(toMoney),
		expenses: {
			food: toMoney(food),
			housing: toMoney(housing),
			health: toMoney(health),
			clothing: toMoney(clothing),
			restaurants: toMoney(restaurants),
			transport: toMoney(transport),
			other: toMoney(other),
		},
		saved: toMoney(draft.saved),
		reserves: draft.reserves.map(toMoney),
		debtPayments: draft.debtPayments.map(toMoney),
		selfRating: draft.selfRating,
		previousMonth: toEstimate(draft.previousMonth),
		twoMonthsAgo: toEstimate(draft.twoMonthsAgo),
	};
};
