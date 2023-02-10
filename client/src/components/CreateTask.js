import React, {useState, useEffect} from 'react'
import { Form, Button } from 'react-bootstrap'
import LeftArrow from '../img/icons8-prev-48.png'

function CreateTask(props) {
  
  const [ime, setIme] = useState("")
  const [opis, setOpis] = useState("")
  const [status, setStatus] = useState(1)
  const [selectedSprint, setSelectedSprint] = useState("");
  const [sprints, setSprints] = useState([]);
  
  const onSubmit = (event) => {
    event.preventDefault()

    if(ime == "" || opis == "")
      return

    if(!selectedSprint)
    {
      alert("Select sprint!");
      return;
    }

    const request = {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' , 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`},
      body: JSON.stringify({'ime' : ime, 'opis' : opis, 'status' : status, "sprint": {"id": selectedSprint}})
    } 

    fetch('https://localhost:7013/Task/CreateTask/' + props.teamID, request).then(response => {
      if(response.ok)
      {
        alert("Task successfully created!");
      }
    })


  }

  useEffect(() => {
    fetch(`https://localhost:7013/Sprint/GetSprintsList/${props.teamID}`)
      .then(response => {
        if(response.ok)
        {
          response.json()
            .then(sprints => {
              setSprints(sprints);
            })
        }
      })
  }, [])
  


  return (
      <Form className='taskForm form2' onSubmit={onSubmit}>

        <Form.Group className='backlog'>

          <h3>Free task list:</h3>
          <div> {props.array1.map( (task) => ( <div className = "TaskOnList" key={task.id} onClick={() => {props.onKlik(task)}}> <img className ="arrowLeft" src={LeftArrow} alt="<" /> <p>{task.ime} - {task.opis}</p></div> ) )} </div> 

        </Form.Group>

        <Form.Group className='form-cont'>
            <Form.Label>Name</Form.Label>

            <Form.Control type='text' placeholder='ime'
                value = {ime} onChange= { (e) => 
                setIme(e.target.value) }/>
        </Form.Group>

        <Form.Group className='form-cont'>
            <Form.Label>Description</Form.Label>

            <Form.Control type='text' placeholder='opis'
                value = {opis} onChange= { (e) => 
                setOpis(e.target.value) }/>
        </Form.Group>

        <Form.Group className='form-cont'>
            <Form.Label>Sprint</Form.Label>

            <Form.Select onChange={(e) => setSelectedSprint(e.target.value)}>
              <option value="">Select sprint</option>
              {sprints.map(sprint => <option value={sprint.id} key={sprint.id}>{sprint.opis}</option>)}
            </Form.Select>
        </Form.Group>

        <Button variant='dark' className='button' type='submit'>Create</Button>

      </Form>

  )
}

export default CreateTask