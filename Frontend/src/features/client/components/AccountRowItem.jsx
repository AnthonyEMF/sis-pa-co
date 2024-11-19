import { Link } from "react-router-dom"

export const AccountRowItem = ({account}) => {
  
  return (
    <tr key={account.id} className="hover:bg-gray-50">
        <td className="px-4 py-2 border-b">{account.code}</td>
        <td className="px-4 py-2 border-b">{account.name}</td>
        <td className="px-4 py-2 border-b">
          <span
            className={`px-5 inline-flex text-xs leading-5 font-semibold rounded-full ${
              account.allowMovement ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"
            }`}
          >
            {account.allowMovement ? "SI" : "NO"}
          </span>
        </td>
        <td className="px-4 py-2 border-b">
          <span
            className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
              account.isActive ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"
            }`}
          >
            {account.isActive ? "Activa" : "Inactiva"}
          </span>
        </td>
        <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
          <Link
            to={`/accounts-details/${account.id}`}
            className="text-blue-600 hover:text-blue-800"
          >
            Ver detalles
          </Link>
        </td>
    </tr>
  )
}