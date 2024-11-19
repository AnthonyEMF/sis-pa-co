import { formatDateShort } from "../../../shared/utils/format-date"

export const LogRowItem = ({log}) => {
  return (
    <tr key={log.id} className="hover:bg-gray-50">
        <td className="px-4 py-2 border-b">{log.action}</td>
        <td className="px-4 py-2 border-b">{log.description}</td>
        <td className="px-4 py-2 border-b">{log.user}</td>
        <td className="px-4 py-2 border-b">{formatDateShort(log.date)}</td>
    </tr>
  )
}

