import { useState } from 'react';
import { Modal, Button, Group } from '@mantine/core';
import Navigation, {navigationItem} from "./Navigation";

interface Props {
  navigationItems: navigationItem[];
  hasKs2Progress: boolean;
  hasProgress8: boolean;
}


export default  function SfbSadMobileNav({navigationItems, hasKs2Progress, hasProgress8}: Props) {
  const [opened, setOpened] = useState(false);
  
  function setClosed() {
    setOpened(false);
  }

  return (
    <>
      <Modal
        opened={opened}
        onClose={() => setOpened(false)}
        fullScreen
        overflow="inside"
        >
        <Navigation navigationItems={navigationItems} onClick={setClosed} hasProgress8={hasProgress8} hasKs2Progress={hasKs2Progress} />
      </Modal>

      <Group position="left">
        <Button variant="subtle" color="#1d70b8" compact onClick={() => setOpened(true)}>
          Change category
        </Button>
      </Group>
    </>
  );
}