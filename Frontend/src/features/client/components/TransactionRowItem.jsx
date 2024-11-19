import { Link } from "react-router-dom"
import { formatDateShort } from "../../../shared/utils/format-date"

export const TransactionRowItem = ({transaction}) => {
  return (
    <tr key={transaction.id}>
        <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
          {transaction.number}
        </td>
        <td className="px-6 py-4 whitespace-nowrap text-sm ">
          {formatDateShort(transaction.date)}
        </td>
        <td className="px-6 py-4 whitespace-nowrap text-sm ">
          {transaction.description}
        </td>
        <td className="px-6 py-4 whitespace-nowrap text-sm">
          {transaction.userName}
        </td>
        <td className="px-6 py-4 whitespace-nowrap text-sm">
          <span
            className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
              transaction.isActive
                ? "bg-green-100 text-green-800"
                : "bg-red-100 text-red-800"
            }`}
          >
            {transaction.isActive ? "Activa" : "Inactiva"}
          </span>
        </td>
        <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
          <Link
            to={`/transactions-details/${transaction.id}`}
            className="text-blue-600 hover:text-blue-800"
          >
            Ver detalles
          </Link>
        </td>
    </tr>
  )
}

