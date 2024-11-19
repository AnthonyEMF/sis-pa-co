import { Navigate, Route, Routes } from "react-router-dom"
import { LoginPage } from "../pages/LoginPage"
import { Nav } from "../../client/components/Nav"

export const SecurityRouter = () => {
  return (
    <div className="overflow-x-hidden bg-gray-400 w-screen h-screen bg-hero-pattern bg-no-repeat bg-cover">
      <Nav />
        <div className="px-6 py-8">
            <div className="container flex justify-between mx-auto">
                <Routes>
                    <Route path='/login' element={<LoginPage/>}/>
                    <Route path='/*' element={<Navigate to={"/login"} />} />
                </Routes>
            </div>
        </div>
    </div>
  )
}

