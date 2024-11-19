import { getMonthName } from "../../../shared/utils/format-date";

export const BalanceRowItem = ({ balance }) => {
  return (
    <tr key={balance.id} className="hover:bg-gray-50">
      <td className="px-4 py-2 border-b">{balance.id}</td>
      <td className="px-4 py-2 border-b">{getMonthName(balance.month)}</td>
      <td className="px-4 py-2 border-b">{balance.year}</td>
      <td className="px-4 py-2 border-b">{balance.accountName}</td>
      <td 
        className={`px-4 font-bold py-2 border-b ${
          balance.balanceAmount < 0 
            ? 'text-red-600' 
            : balance.balanceAmount === 0 
            ? 'text-blue-600' 
            : 'text-green-600'
        }`}
      >
        {balance.balanceAmount}$
      </td>
    </tr>
  );
};
